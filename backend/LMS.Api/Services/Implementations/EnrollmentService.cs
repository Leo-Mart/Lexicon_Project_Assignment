using AutoMapper;
using LMS.Api.Constants;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Course;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<CourseDto?> GetStudentCourseAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        Enrollment? enrollment =
            await _enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);

        if (enrollment is null)
        {
            return null;
        }

        return _mapper.Map<CourseDto>(enrollment.Course);
    }

    public async Task<List<Enrollment>> GetEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _enrollmentRepository.GetByCourseIdAsync(courseId, cancellationToken);
    }

    public async Task<bool> AssignOrChangeCourseAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        User? student = await _userManager.FindByIdAsync(studentId.ToString());

        if (student is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        IList<string> roles = await _userManager.GetRolesAsync(student);

        if (!roles.Contains(RoleConstants.Student))
        {
            throw new InvalidOperationException("Course can only be assigned to a student.");
        }

        Course? course = await _courseRepository.GetCourseByIdAsync(courseId);

        if (course is null)
        {
            throw new KeyNotFoundException("Course not found.");
        }

        Enrollment? enrollment = await _enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);

        if (enrollment is null)
        {
            enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        }
        else
        {
            enrollment.CourseId = courseId;
            enrollment.Course = course;

            _enrollmentRepository.Update(enrollment);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task RemoveCourseAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        Enrollment? enrollment =
            await _enrollmentRepository.GetByStudentIdAsync(
                studentId,
                cancellationToken);

        if (enrollment is null)
        {
            return;
        }

        _enrollmentRepository.Delete(enrollment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

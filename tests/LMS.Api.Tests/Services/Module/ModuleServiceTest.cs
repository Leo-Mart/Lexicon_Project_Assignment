using AutoMapper;
using LMS.Api.DTOs.Module;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces.Course;
using LMS.Api.Repositories.Interfaces.Module;
using LMS.Api.Services.Implementations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ModuleEntity = LMS.Api.Models.Module;

namespace LMS.Api.Tests.Services.Module
{
    public class ModuleServiceTests
    {
        private readonly Mock<IModuleRepository> _mockModuleRepo;
        private readonly Mock<ICourseRepository> _mockCourseRepo;
        private readonly ModuleService _service;

        public ModuleServiceTests()
        {
            _mockModuleRepo = new Mock<IModuleRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();

            IMapper mapper = new MapperConfiguration(
                cfg => cfg.AddProfile<ModuleProfile>(),
                NullLoggerFactory.Instance
            ).CreateMapper();

            _service = new ModuleService(_mockModuleRepo.Object, _mockCourseRepo.Object, mapper);
        }

        [Fact]
        public async Task CreateModule_WithValidDate_ShouldReturnCreatedModule()
        {
            Guid moduleId = Guid.NewGuid();
            Guid courseId = Guid.NewGuid();

            var request = new CreateNewModuleDto
            {
                CourseId = courseId,
                Name = "A good module",
                Description = "A Description",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
            };

            _mockModuleRepo
                .Setup(r => r.CreateModuleAsync(It.IsAny<ModuleEntity>()))
                .ReturnsAsync(
                    (ModuleEntity m) =>
                    {
                        m.ModuleId = moduleId;
                        return m;
                    }
                );

            var result = await _service.CreateNewModule(request);

            Assert.NotNull(result);
            Assert.Equal(moduleId, result.ModuleId);
            Assert.Equal(request.Name, result.Name);
        }
    }
}

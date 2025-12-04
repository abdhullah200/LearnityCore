using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;

namespace LSC.OnlineCourse.API.Common
{
    /// <summary>
    /// Provides mapping configurations between domain models and data transfer objects (DTOs) for use in the
    /// application. This class defines mappings for various entities, including videos, courses, enrollments, payments,
    /// reviews, and instructors.
    /// </summary>
    /// <remarks>This class inherits from the <see cref="Profile"/> class provided by AutoMapper and is used
    /// to configure object-object mappings. The mappings defined here include custom transformations and ignore rules
    /// where necessary to handle specific use cases, such as formatting user names or excluding certain properties from
    /// mapping.</remarks>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Configures object-object mappings for the application using AutoMapper.
        /// </summary>
        /// <remarks>This profile defines mappings between various domain models and their corresponding
        /// view models or DTOs. It includes custom mapping configurations for specific properties where necessary, such
        /// as formatting user names or selecting specific related entities. The mappings are bidirectional where
        /// appropriate.</remarks>
        public MappingProfile()
        {
            CreateMap<VideoRequest, VideoRequestModel>()
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName}, {src.User.LastName}"));

            CreateMap<VideoRequestModel, VideoRequest>()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // We don't map User here since it's handled separately

            CreateMap<CourseEnrollmentModel, Enrollment>();
            CreateMap<Enrollment, CourseEnrollmentModel>()
    .ForMember(dest => dest.CoursePaymentModel,
        opt => opt.MapFrom(src =>
            src.Payments.OrderByDescending(o => o.PaymentDate).FirstOrDefault()))
    .ForMember(dest => dest.CourseTitle,
        opt => opt.MapFrom(src => src.Course.Title));  // Mapping for CourseTitle


            CreateMap<CoursePaymentModel, Payment>();
            CreateMap<Payment, CoursePaymentModel>();

            CreateMap<Review, UserReviewModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.LastName}, {src.User.FirstName}"));

            CreateMap<UserReviewModel, Review>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());

            CreateMap<InstructorModel, Instructor>();
            CreateMap< Instructor, InstructorModel>();
        }
    }
}

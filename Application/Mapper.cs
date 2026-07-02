using AutoMapper;
using WorkoutTrackerApi.Domain;

namespace WorkoutTrackerApi.Application;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<CreatePlanDto, Plan>();
        CreateMap<UpdatePlanDto, Plan>();
        CreateMap<Plan, PlanResponse>();

        CreateMap<RegisterUserDto, User>();
        

        CreateMap<Exercise, ExerciseResponse>();
    }
}
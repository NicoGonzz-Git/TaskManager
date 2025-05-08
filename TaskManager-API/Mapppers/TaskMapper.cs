using AutoMapper;
using TaskManager_API.DTOs;
using TaskManager_API.Models.Domain;

namespace TaskManager.Mappings
{
    public class TaskMapperProfile : Profile
    {
        public TaskMapperProfile()
        {
            CreateMap<TaskItem, TasksDto>();
            CreateMap<TaskItem, TaskItem>();
            CreateMap<TaskResponseDto, TasksDto>();
            CreateMap<CreateTask, TaskItem>();
            CreateMap<UpdateTask, TaskItem>();
        }
    }
}
using GenAiPoc.Core.DTOs;
using GenAiPoc.Core.Entities.Templates;
using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenAiPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            this._templateService = templateService;
        }

        [HttpPost("CreateTemplateCategory")]
        public async Task<Response<bool>> CreateTemplateCategoryAsync([FromBody] CreateTemplateCategoryDto dto)
        {
            var response = await _templateService.CreateTemplateCategoryAsync(dto);
            return response;
        }

        [HttpGet("GetAllTemplateCategories")]
        public async Task<ResponseList<TemplateCategoryDto>> GetAllTemplateCategoriesAsync()
        {
            var response = await _templateService.GetAllTemplateCategoryAsync();
            return response;
        }

        [HttpPost("CreateTemplate")]
        public async Task<Response<bool>> CreateTemplateAsync([FromBody] CreateTemplateDto templateDto)
        {
            var response = await _templateService.CreateTemplateAsync(templateDto);
            return response;
        }

        [HttpGet("GetAllTemplates")]
        public async Task<ResponseList<TemplateDto>> GetAllTemplatesAsync(int page, int pageSize, string? searchTerm)
        {
            var response = await _templateService.GetAllTemplatesAsync(page, pageSize, searchTerm);
            return response;
        }

        [HttpGet("GetTemplateById")]
        public async Task<Response<TemplateDto>> GetTemplateByIdAsync(int id)
        {
            var response = await _templateService.GetTemplateByIdAsync(id);
            return response;
        }

        [HttpGet("GetTemplateByCategoryId")]
        public async Task<ResponseList<TemplateDto>> GetTemplatesByCategoryIdAsync(int categoryId)
        {
            var response = await _templateService.GetTemplatesByCategoryIdAsync(categoryId);
            return response;
        }

        [HttpPost("UpdateUserTemplate")]
        public async Task<Response<bool>> UpdateUserTemplateAsync([FromForm] UpdateTemplateDto templateDto)
        {
            var response = await _templateService.UpdateTemplateAsync(templateDto);
            return response;
        }


        //Start Template Section APIs
        [HttpPost("CreateTemplateSection")]
        public async Task<Response<bool>> CreateTemplateSectionAsync([FromBody] TemplateSectionDTO templateSectionDto)
        {
            return await _templateService.CreateTemplateSectionAsync(templateSectionDto);
        }

        [HttpGet("GetAllTemplateSectionByTemplateId")]
        public async Task<ResponseList<TemplateSectionDTO>> GetAllTemplateSectionByTemplateIdAsync(int templateId)
        {
            return await _templateService.GetAllTemplateSectionByTemplateIdAsync(templateId);
        }

        [HttpPut("UpdateTemplateSection")]
        public async Task<Response<bool>> UpdateTemplateSectionAsync([FromBody] TemplateSectionDTO templateSectionDto)
        {
            return await _templateService.UpdateTemplateSectionAsync(templateSectionDto);
        }

        [HttpDelete("DeleteTemplateSection")]
        public async Task<Response<bool>> DeleteTemplateSectionAsync(int id)
        {
            return await _templateService.DeleteTemplateSectionAsync(id);
        }
        //End Template Section APIs


        //Start of UserTemplates APIs

        [HttpPost("CreateUserTemplate")]
        public async Task<Response<bool>> CreateUserTemplate([FromForm] CreateUserTemplateDto dto)
        {
            return await _templateService.CreateUserTemplateAsync(dto);
        }

        [HttpGet("GetAllUserTemplate")]
        public async Task<PaginatedResponse<UserTemplateSpecificationDto>> GetAllUserTemplate([FromQuery] UserTemplateFilterDto filter)
        {
            return await _templateService.GetAllUserTemplateAsync(filter);
        }

        //[HttpGet("GetAllbyUserTemplateCategory")]
        //public async Task<ResponseList<UserTemplateDto>> GetAllByCategory([FromQuery] int categoryId)
        //{
        //    return await _templateService.GetAllByCategoryAsync(categoryId);
        //}

        [HttpPut("UpdateTemplate")]
        public async Task<Response<bool>> UpdateTemplate([FromBody] UpdateUserTemplateDto dto)
        {
            return await _templateService.UpdateUserTemplateAsync(dto);
        }

        [HttpGet("GetAllTemplateSections")]
        public async Task<ResponseList<TemplateSectionDto>> GetAllTemplateSubSectionsAsync()
        {
            var response = await _templateService.GetAllTemplateSubSectionsAsync();
            return response;
        }

        //end of UserTemplates


        [HttpPost("UseTemplate")]
        public async Task<Response<bool>> CreateUserTemplateFrequencyAsync([FromBody] CreateUserTemplateFrequencyDto dto)
        {
            var response = await _templateService.CreateUserTemplateFrequencyAsync(dto);
            return response;
        }

        [HttpDelete("DeleteUserTemplate")]


        public async Task<Response<bool>> DeleteUserTemplatesAsync(DeleteUserTemplateDto deleteUser)
        {
            var response = await _templateService.DeleteUserTemplatesAsync(deleteUser);
            return response;
        }

    }
}

using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class SectionRepo : ISectionRepo
{
    private readonly AppDbContext _appDbContext;

    public SectionRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    ResponseResult result = new ResponseResult();
    public async Task<List<Section>> GetAllSectonsAsync()
    {
        return await _appDbContext.Sections
                       .OrderBy(c => c.SectionId)
                       .Where(s => s.Isdeleted == false)
                       .ToListAsync();
    }
    public async Task<ResponseResult> AddSectionAsync(Section section)
    {
        try
        {
            int sectionCount = await _appDbContext.Sections.CountAsync();
            if (section == null)
            {
                result.Message = "Section Data not found in Repo";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                section.SectionId = sectionCount + 1;
                _appDbContext.Sections.Add(section);
                await _appDbContext.SaveChangesAsync();
                result.Data = section.SectionId;
                result.Message = "Successfully saved New Section";
                result.Status = ResponseStatus.Success;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> UpdateSectionAsync(Section updateSection)
    {
        try
        {
            _appDbContext.Sections.Update(updateSection);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Successfully Update Section";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<Section> GetSectionAsync(int sectionId)
    {
        return await _appDbContext.Sections.Where(s => s.SectionId == sectionId).FirstOrDefaultAsync();
    }
}

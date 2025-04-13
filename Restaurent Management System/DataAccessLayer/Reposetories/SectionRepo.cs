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
    public async Task<List<SectionDetails>> GetAllSectonsAsync()
    {
        return await _appDbContext.Sections
                       .OrderBy(c => c.SectionId)
                       .Where(s => s.Isdeleted == false)
                       .Select(c => new SectionDetails { SectionId = c.SectionId, SectionName = c.SectionName, Description = c.Description })
                       .ToListAsync();
    }
    public async Task<ResponseResult> DeleteSectionBySectionId(int id,int editorId)
    {
        var Section = await _appDbContext.Sections.FirstOrDefaultAsync(i => i.SectionId == id);
        if (Section == null)
        {
            result.Message = "Section is not Found";
            result.Status = ResponseStatus.NotFound;
            return result;
        }
        Section.Isdeleted = true;
        Section.Modifiedby = editorId;
        Section.Modifiedat = DateTime.Now;
        await _appDbContext.SaveChangesAsync();
        result.Message = "Section is successfully deleted";
        result.Status = ResponseStatus.Success;
        return result;

    }
    public async Task<ResponseResult> AddSectionAsync(string SectionName, string description)
    {
        try
        {
            var query = await _appDbContext.Sections.Where(c => c.SectionName.ToLower() == SectionName.ToLower()).FirstOrDefaultAsync();
            //At Database by mistaken section id store as integer instead of serial
            int sectionCount = await _appDbContext.Sections.CountAsync();
            if (query == null)
            {
                Section newSection = new Section
                {
                    SectionId = sectionCount+1,
                    SectionName = SectionName,
                    Description = description,
                    Createdat = DateTime.Now,
                    Createdby = 1,
                    Isdeleted = false
                };
                _appDbContext.Sections.Add(newSection);
                await _appDbContext.SaveChangesAsync();
                result.Message = "Successfully saved New Section";
                result.Status = ResponseStatus.Success;
                return result;
            }
            result.Message = "Section already Exist";
            result.Status = ResponseStatus.NotFound;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<ResponseResult> EditSectionAsync(int sectionId, string sectionName, string description,int editorId)
    {
        try
        {
            var query = await _appDbContext.Sections.Where(c => c.SectionId == sectionId).FirstOrDefaultAsync();
            if (query == null)
            {
                result.Message = "Section is not found for Update";
                result.Status = ResponseStatus.NotFound;
                // return result;
            }
            else
            {
                query.SectionName = sectionName;
                query.Description = description;
                query.Modifiedat = DateTime.Now;
                query.Modifiedby = editorId;
                query.Isdeleted = false;

                _appDbContext.Sections.Update(query);
                await _appDbContext.SaveChangesAsync();
                
                result.Message = "Section Update Successfully";
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

    public async Task<Section> GetSectionAsync(int sectionId){
        return await _appDbContext.Sections.Where(s=>s.SectionId == sectionId).FirstOrDefaultAsync();
    }   
}

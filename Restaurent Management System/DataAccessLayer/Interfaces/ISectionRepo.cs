using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces;

public interface ISectionRepo
{
    Task<List<SectionDetails>> GetAllSectonsAsync();
    Task<ResponseResult> DeleteSectionBySectionId(int id,int editorId);
    Task<ResponseResult> AddSectionAsync(string SectionName, string description);
    Task<ResponseResult> EditSectionAsync(int sectionId, string sectionName, string description,int editorId);
    Task<Section> GetSectionAsync(int sectionId);
}

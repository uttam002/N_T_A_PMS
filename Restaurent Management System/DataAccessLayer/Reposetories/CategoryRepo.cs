using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories;

public class CategoryRepo : ICategoryRepo
{
    private readonly AppDbContext _appDbContext;

    public CategoryRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    ResponseResult result = new ResponseResult();

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _appDbContext.Categories
                        .OrderBy(c => c.CategoryId)
                        .Where(a => a.Isactive == true)
                        .ToListAsync();
    }
    public async Task<ResponseResult> AddCategory(Category newCategory)
    {
        try
        {
            _appDbContext.Categories.Add(newCategory);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Successfully saved New Category";
            result.Status = ResponseStatus.Success;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    public async Task<Category> GetCategoryByNameAsync(string categoryName)
    {
        try
        {
            Category category = await _appDbContext.Categories.Where(c => c.CategoryName.ToLower() == categoryName.ToLower()).FirstOrDefaultAsync();
            return category;
        }
        catch
        {
            return null;
        }
    }
    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        try
        {
            Category category = await _appDbContext.Categories.Where(c => c.CategoryId == categoryId).FirstOrDefaultAsync();
            return category;
        }
        catch
        {
            return null;
        }
    }
    public async Task<ResponseResult> UpdateCategory(Category newCategory)
    {
        try
        {
            _appDbContext.Categories.Update(newCategory);
            await _appDbContext.SaveChangesAsync();
            result.Message = "Successfully Updated Category";
            result.Status = ResponseStatus.Success;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    // public async Task<ResponseResult> DeleteCategoryByCategoryIdAsync(int id,int editorId)
    // {
    //     Category category = await _appDbContext.Categories.FirstOrDefaultAsync(i => i.CategoryId == id);
    //     if (category == null)
    //     {
    //         result.Message = "Category is not Found";
    //         result.Status = ResponseStatus.NotFound;
    //         return result;
    //     }
    //     category.Isactive = false;
    //     category.Modifyby = editorId;
    //     category.Modifyat = DateTime.Now;
    //     await _appDbContext.SaveChangesAsync();
    //     result.Message = "Category is successfully deleted";
    //     result.Status = ResponseStatus.Success;
    //     return result;

    // }
}

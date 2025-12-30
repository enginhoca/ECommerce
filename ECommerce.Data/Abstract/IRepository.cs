using System;
using System.Linq.Expressions;

namespace ECommerce.Data.Abstract;

public interface IRepository<T> where T : class
{
    /// <summary>
    /// ID'ye göre tek bir nesne getirme
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetAsync(int id);

    /// <summary>
    /// Filtreleyerek tek bir nesne getirme
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="showIsDeleted"></param>
    /// <param name="asExpanded"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<T> GetAsync(
        Expression<Func<T, bool>> predicate,
        bool showIsDeleted = false,
        bool asExpanded = false,
        params Func<IQueryable<T>, IQueryable<T>>[] includes
    );


    /// <summary>
    /// Tüm nesneleri getirme
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync();


    /// <summary>
    /// Filtreleyerek bir entity koleksiyonu getirir.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="orderBy"></param>
    /// <param name="showIsDeleted"></param>
    /// <param name="asExpanded"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>> predicate = null!,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!,
        bool showIsDeleted = false,
        bool asExpanded = false,
        params Func<IQueryable<T>, IQueryable<T>>[] includes
    );

    /// <summary>
    /// Entitydeki kayıt sayısını verir.
    /// </summary>
    /// <returns></returns>
    Task<int> CountAsync();

    /// <summary>
    /// Filtereyelerek entitydeki kayıt sayısını verir.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Verilen kritere uygun kayıt var mı yok mu?
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Yeni bir nesne ekleme
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Bir nesneyi güncelleme
    /// </summary>
    /// <param name="entity"></param>
    void Update(T entity);

    /// <summary>
    /// Toplu güncelleme yapar.
    /// </summary>
    /// <param name="entities"></param>
    void BatchUpdate(IEnumerable<T> entities);

    /// <summary>
    /// Bir nesneyi silme
    /// </summary>
    /// <param name="entity"></param>
    void Remove(T entity);

    /// <summary>
    /// Sayfalanmış veri getirme
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="orderBy"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="showIsDeleted"></param>
    /// <param name="asExpanded"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int skip = 0,
        int take = 10,
        bool showIsDeleted = false,
        bool asExpanded = false,
        params Func<IQueryable<T>, IQueryable<T>>[] includes
    );
}

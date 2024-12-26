namespace ELibrary.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStaffRepository StaffRepository { get; }
        
        IMemberRepository MemberRepository { get; }
        
        IPhoneRepository PhoneRepository { get; }
        
        IBookRepository BookRepository { get; }
        
        IBookAuthorRepository BookAuthorRepository { get; }
        
        IAuthorRepository AuthorRepository { get; }
        
        ILoanRepository LoanRepository { get; }

        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
        
        Task SaveChangesAsync();
    }
}
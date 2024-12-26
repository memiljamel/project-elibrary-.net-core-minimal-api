using ELibrary.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ELibrary.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        private IStaffRepository _staffRepository;
        private IMemberRepository _memberRepository;
        private IPhoneRepository _phoneRepository;
        private IBookRepository _bookRepository;
        private IBookAuthorRepository _bookAuthorRepository;
        private IAuthorRepository _authorRepository;
        private ILoanRepository _loanRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IStaffRepository StaffRepository
        {
            get
            {
                if (_staffRepository == null)
                {
                    _staffRepository = new StaffRepository(_context);
                }

                return _staffRepository;
            }
        }

        public IMemberRepository MemberRepository
        {
            get
            {
                if (_memberRepository == null)
                {
                    _memberRepository = new MemberRepository(_context);
                }

                return _memberRepository;
            }
        }

        public IPhoneRepository PhoneRepository
        {
            get
            {
                if (_phoneRepository == null)
                {
                    _phoneRepository = new PhoneRepository(_context);
                }

                return _phoneRepository;
            }
        }

        public IBookRepository BookRepository
        {
            get
            {
                if (_bookRepository == null)
                {
                    _bookRepository = new BookRepository(_context);
                }

                return _bookRepository;
            }
        }

        public IBookAuthorRepository BookAuthorRepository
        {
            get
            {
                if (_bookAuthorRepository == null)
                {
                    _bookAuthorRepository = new BookAuthorRepository(_context);
                }

                return _bookAuthorRepository;
            }
        }

        public IAuthorRepository AuthorRepository
        {
            get
            {
                if (_authorRepository == null)
                {
                    _authorRepository = new AuthorRepository(_context);
                }

                return _authorRepository;
            }
        }

        public ILoanRepository LoanRepository
        {
            get
            {
                if (_loanRepository == null)
                {
                    _loanRepository = new LoanRepository(_context);
                }

                return _loanRepository;
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
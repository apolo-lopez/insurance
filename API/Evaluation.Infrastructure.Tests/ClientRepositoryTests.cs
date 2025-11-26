using Evaluation.Domain.Entities;
using Evaluation.Domain.ValueObjects;
using Evaluation.Infrastructure.Data;
using Evaluation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Infrastructure.Tests
{
    public class ClientRepositoryTests
    {
        private readonly Mock<AppDbContext> _dbContextMock;
        private readonly Mock<DbSet<Client>> _dbSetMock;
        private readonly ClientRepository _clientRepository;

        [Fact]
        public async Task AddAsync_CallAddAsyncOnDbSet()
        {
            var identificationNumber = new IdentificationNumber("1234567890"); // Asume un constructor válido en IdentificationNumber
            var client = new Client(
                identificationNumber,
                "Test Client",
                "test@example.com",
                "555-1234",
                "Some Address",
                "user-id-123"
            );
            await _clientRepository.AddAsync(client);
            _dbSetMock.Verify(dbSet => dbSet.AddAsync(client, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenClientExists()
        {
            var clientId = Guid.NewGuid();
            var identificationNumber = new IdentificationNumber("1234567890"); // Asume un constructor válido en IdentificationNumber
            var client = new Client(
                identificationNumber,
                "Test Client",
                "test@example.com",
                "555-1234",
                "Some Address",
                "user-id-123"
             );

            var data = new List<Client> { client }.AsQueryable();

            _dbSetMock.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            _dbSetMock.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            _dbSetMock.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _dbSetMock.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var exists = await _clientRepository.ExistsAsync(clientId);

            Assert.True(exists);
        }
    }
}

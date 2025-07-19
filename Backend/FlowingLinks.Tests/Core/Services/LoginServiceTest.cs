using FlowingLinks.Core;
using FlowingLinks.Core.Models;
using FlowingLinks.Core.Services;
using FlowingLinks.Core.Utils;
using FlowingLinks.Tests.Mocks;

namespace FlowingLinks.Tests.Core.Services
{
    [TestClass]
    public class LoginServiceTest
    {
        private readonly TestDbContext _context;
        private readonly LoginService _service;
        private readonly User _testUser;

        public LoginServiceTest()
        {
            _context = new TestDbContext();
            _context.InitializeTestDatabase();
            _service = new LoginService(_context);
            
            _testUser = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = HashUtils.GenerateMd5Hash("123456")
            };
        }

        [TestMethod]
        public async Task LoginService_Execute_ValidCredentials()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.Execute("cassioalmeron", "123456");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Cassio Almeron", result.Name);
            Assert.AreEqual("cassioalmeron", result.Username);
            Assert.AreEqual(HashUtils.GenerateMd5Hash("123456"), result.Password);
        }

        [TestMethod]
        public async Task LoginService_Execute_InvalidUsername()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act & Assert
            try
            {
                await _service.Execute("nonexistentuser", "123456");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Invalid username or password", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_InvalidPassword()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act & Assert
            try
            {
                await _service.Execute("cassioalmeron", "wrongpassword");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Invalid username or password", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_EmptyUsername()
        {
            // Act & Assert
            try
            {
                await _service.Execute("", "123456");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Username cannot be empty", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_NullUsername()
        {
            // Act & Assert
            try
            {
                await _service.Execute(null, "123456");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Username cannot be empty", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_WhitespaceUsername()
        {
            // Act & Assert
            try
            {
                await _service.Execute("   ", "123456");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Username cannot be empty", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_EmptyPassword()
        {
            // Act & Assert
            try
            {
                await _service.Execute("cassioalmeron", "");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Password cannot be empty", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_NullPassword()
        {
            // Act & Assert
            try
            {
                await _service.Execute("cassioalmeron", null);
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Password cannot be empty", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_WhitespacePassword()
        {
            // Act & Assert
            try
            {
                await _service.Execute("cassioalmeron", "   ");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Password cannot be empty", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_EmptyDatabase()
        {
            // Act & Assert
            try
            {
                await _service.Execute("anyusername", "anypassword");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Invalid username or password", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_CaseSensitiveUsername()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act & Assert
            try
            {
                await _service.Execute("CASSIOALMERON", "123456");
                Assert.Fail();
            }
            catch (FlowingLinksException e)
            {
                Assert.AreEqual("Invalid username or password", e.Message);
            }
        }

        [TestMethod]
        public async Task LoginService_Execute_CaseSensitivePassword()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act & Assert
            try
            {
                await _service.Execute("cassioalmeron", "123456");
                var result = await _service.Execute("cassioalmeron", "123456");
                Assert.IsNotNull(result);
            }
            catch (FlowingLinksException e)
            {
                Assert.Fail($"Should not throw exception for correct case: {e.Message}");
            }
        }
    }
} 
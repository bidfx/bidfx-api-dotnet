using Moq;

namespace BidFX.Public.API.Price.Tools
{
    public class TestUtils
    {
        internal static LoginService GetMockLoginService()
        {
            Mock<LoginService> loginService = new Mock<LoginService>();
            loginService.Setup(ls => ls.LoggedIn).Returns(true);
            return loginService.Object;
        }
    }
}
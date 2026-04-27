using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodly.Tests
{
    // Fomat : [Tên phương thức muốn test ]_[Kết quả mong đợi]_[Điều kiện thực hiện test(khi tham số là null,khi người dùng tự follow chính mình, khi database bị ngắt kết nối...)] 
    // SetUp : giả lập dữ liệu mặc định trả về khi gọi 1 method của dependency 
    // Verify() dùng để xác nhận một method của dependency có được gọi hay không.
    public class UserServiceTests
    {
        [Fact]
        public void UserRegisterNewAccount_ShouldRegisterSuccessfully_WhenUserFillCorrectly()
        {
            // arrage 



            //act 



            // assert 



            Assert.True(true);
        }

    }
}

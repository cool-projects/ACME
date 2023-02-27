using Acme.Data;
using Helpers;
using IBusinessServices;
using Models;

namespace BusinessServices
{
    public class AccountService : IAccountService
    {
		private readonly AcmeDBContext db;
        private readonly IEmployeeService _employee;
        private readonly ILoggingHelper _logging;
		public AccountService(AcmeDBContext dataContext, IEmployeeService employee, ILoggingHelper loggingHelper)
		{
			db = dataContext;
			_employee = employee;
			_logging = loggingHelper;
		}

		public BaseResponse Register(RegisterModel model)
		{
			int userId = 0;
			var response = new BaseResponse();
			var errors = new List<string>();
			try
			{
				
				//save user
				var checkUser = db.Employees.Where(x => x.Email == model.Email).FirstOrDefault();
				if (checkUser == null)
				{
                    //save employee
                    var employee = new EmployeeModel()
                    {
                        Age = model.Age,
                        Email = model.Email,
                        GenderId = model.GenderId,
                        Name = model.Name,
						ReportingLine = model.ReportingLine,
                    };
                    var saveEmployeeResponse = _employee.SaveEmployee(employee);

                    var _errors = response.Errors;
					response = CreateUserLogin(new UserLoginModel() { Username = model.Email, Password = model.Password }, ref _errors);

					if (response.Success)
						response.Success = AddUserRole(saveEmployeeResponse.EmployeeId, model.RoleId);
				}
				else
				{
					errors.Add($"Employee account already exist for this employee");
					response.Errors = errors;
					response.Success = false;
				}
			}
			catch (Exception ex)
			{
				errors.Add($"There was an error encountered when registering employee.");
				response.Errors = errors;
				response.Success = false;
				_logging.ExceptionLogger(ex.Message, ex.StackTrace);
			}
			return response;
		}

		private BaseResponse CreateUserLogin(UserLoginModel user, ref List<string> errors)
		{
			var response = new BaseResponse();
			response.Errors = errors;
			try
			{
				var checkUser = db.UserLogins.Where(x => x.UserName == user.Username).FirstOrDefault();
				if (checkUser != null)
				{
					response.Success = false;
					errors.Add($"Username {user.Username} is taken. Please use another username or proceed to login.");
					response.Errors = errors;
				}
				else
				{
					var salt = PasswordHelper.GetSecureSalt();

					var passwordHashed = PasswordHelper.HashUsingPbkdf2(user.Password, salt);
					var passwordSalt = Convert.ToHexString(salt);
					var userLogin = new UserLogin()
					{
						UserName = user.Username,
						Password = passwordHashed,
						PasswordSalt = passwordSalt,
						DateCreated = DateTime.Now
					};
					db.UserLogins.Add(userLogin);
					db.SaveChanges();

					response.Success = true;
				}
			}
			catch (Exception ex)
			{
				errors.Add($"There was an error encountered when creating user login.");
				response.Errors = errors;
				response.Success = false;
				_logging.ExceptionLogger(ex.Message, ex.StackTrace);
			}
			return response;
		}

        public List<RoleModel> GetRoles()
        {
			var response = new List<RoleModel>();

			var roles = db.Roles.ToList();
            foreach (var role in roles)
            {
				var model = new RoleModel()
				{
					RoleId = role.RoleId,
					RoleName = role.RoleDescription
				};
				response.Add(model);
            }
			return response;
        }

		public UserModel Login(UserLoginModel user)
		{
			UserModel? response = new UserModel();

			try
			{
				
				var loginUser = db.UserLogins.Where(x => x.UserName == user.Username).FirstOrDefault();
				if (loginUser != null)
				{
                    var passwordValid = PasswordHelper.ValidatePassword(user.Password, loginUser.Password, loginUser.PasswordSalt);
                    if (passwordValid)
                    {
                        loginUser.DateLastLogin = DateTime.Now;
                        db.SaveChanges();

						var employeeId = db.Employees.Where(x => x.Email == user.Username).FirstOrDefault().EmployeeId;
                        response = GetUserDetailsByUserId(employeeId);
                        response.Success = true;
                    }
                    response.Errors = new List<string>() { "Invalid username or password." };
                }
                else
                {
					response.Success = false;
					response.Errors = new List<string>() { "Invalid username or password." };
				}
			}
			catch (Exception ex)
			{
				_logging.ExceptionLogger(ex.Message, ex.StackTrace);
			}
			return response;
		}

		public UserModel GetUserDetailsByUserId(int userId)
		{
			UserModel? response = new UserModel();
			try
			{
				var user = _employee.GetEmployee(userId);
				var userRole = GetUserRoles(user.EmployeeId);
				if (user != null)
				{
					response = new UserModel()
					{
						UserId = user.EmployeeId,
						Name = user.Name,
						Email = user.Email,
						UserRoles = userRole,
						UserName = user.Email
					};
					response.Success = true;
				}
			}
			catch (Exception ex)
			{
				_logging.ExceptionLogger(ex.Message, ex.StackTrace);
			}

			return response;
		}

		public List<UserRoleModel> GetUserRoles(int userid)
		{
			List<UserRoleModel> listUserRoles = new List<UserRoleModel>();
			foreach (var item in db.UserRoles.Where(x => x.EmployeeId == userid))
			{
				var userRole = new UserRoleModel()
				{
					UserRoleId = item.UserRoleId,
					RoleId = item.RoleId,
					UserId = item.EmployeeId,
					Role = new RoleModel()
					{
						RoleId = item.RoleId,
						RoleName = db.Roles.Where(x => x.RoleId == item.RoleId).FirstOrDefault()?.RoleDescription
					}
				};

				listUserRoles.Add(userRole);
			}
			return listUserRoles;
		}

		private bool AddUserRole(int userId, int roleId)
		{
			bool response = true;
			var errors = new List<string>();
			try
			{
				if (roleId == 0)
				{
					return response;
				}
				var checkUser = db.Employees.Where(x => x.EmployeeId == userId).ToList();
				if (checkUser.Count == 0)
				{
					return response;
				}
				var checkUserRole = db.UserRoles.Where(x => x.EmployeeId == userId && x.RoleId == roleId).ToList();
				if (checkUserRole.Count > 0) //
				{
					return response;
				}
				else
				{
					var userRoleNew = new UserRole()
					{
						EmployeeId = userId,
						RoleId = roleId
					};
					db.UserRoles.Add(userRoleNew);
					db.SaveChanges();

					response = true;
				}
			}
			catch (Exception ex)
			{
				response = false;
				_logging.ExceptionLogger(ex.Message, ex.StackTrace);
			}
			return response;
		}

		public BaseResponse UpdateUserRole(int userId, int roleId)
		{
			var response = new BaseResponse();
			var userRole = db.UserRoles.Where(x => x.EmployeeId == userId && x.RoleId == roleId).FirstOrDefault();
			if(userRole != null)
			{
				userRole.EmployeeId = userId;
				userRole.RoleId = roleId;
				db.SaveChanges();
				response.Success = true;
			}
			else
			{
				response.Success = false;
			}
			return response;
		}

    }
}
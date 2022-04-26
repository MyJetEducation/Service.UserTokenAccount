using System;
using System.Text.Json;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Services
{
	public interface IUserFirstLoginRepository
	{
		ValueTask<bool> WasLogin(string userId);
		ValueTask<bool> Save(string userId, bool waslogin);
	}

	public class UserFirstLoginRepository : IUserFirstLoginRepository
	{
		private readonly Func<string> _firstLoginKey = Program.ReloadedSettings(model => model.KeyFirstLogin);

		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public UserFirstLoginRepository(IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService) => _serverKeyValueService = serverKeyValueService;

		public async ValueTask<bool> WasLogin(string userId)
		{
			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				Key = _firstLoginKey.Invoke(),
				UserId = userId
			});

			string value = response?.Value;
			if (value == null)
				return false;

			return JsonSerializer.Deserialize<UserFirstLoginDto>(value)?.WasLogin == true;
		}

		public async ValueTask<bool> Save(string userId, bool waslogin)
		{
			CommonGrpcResponse commonGrpcResponse = await _serverKeyValueService.TryCall(service => service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = _firstLoginKey.Invoke(),
						Value = JsonSerializer.Serialize(new UserFirstLoginDto {WasLogin = waslogin})
					}
				}
			}));

			return commonGrpcResponse.IsSuccess;
		}
	}
}
using System.Text.Json;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;

namespace Service.UserTokenAccount.Services
{
	public interface IServerKeyValueDtoRepository<TDto> where TDto : class
	{
		ValueTask<TDto> Get(string key, string userId);
		ValueTask<bool> Save(string key, string userId, TDto dto);
	}

	public class ServerKeyValueDtoRepository<TDto> : IServerKeyValueDtoRepository<TDto> where TDto : class
	{
		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public ServerKeyValueDtoRepository(IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService) => _serverKeyValueService = serverKeyValueService;

		public async ValueTask<TDto> Get(string key, string userId)
		{
			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				Key = key,
				UserId = userId
			});

			string value = response?.Value;

			return value == null
				? null
				: JsonSerializer.Deserialize<TDto>(value);
		}

		public async ValueTask<bool> Save(string key, string userId, TDto dto)
		{
			CommonGrpcResponse commonGrpcResponse = await _serverKeyValueService.TryCall(service => service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = key,
						Value = JsonSerializer.Serialize(dto)
					}
				}
			}));

			return commonGrpcResponse.IsSuccess;
		}
	}
}
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
	public interface ITutorialProgressPrcRepository
	{
		ValueTask<TutorialProgressPrcDto[]> Get(string userId);
		ValueTask<bool> Save(string userId, TutorialProgressPrcDto[] dtos);
	}

	public class TutorialProgressPrcRepository : ITutorialProgressPrcRepository
	{
		private readonly Func<string> _tutorialProgressPrcKey = Program.ReloadedSettings(model => model.KeyTutorialProgressPrc);

		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public TutorialProgressPrcRepository(IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService) => _serverKeyValueService = serverKeyValueService;

		public async ValueTask<TutorialProgressPrcDto[]> Get(string userId)
		{
			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				Key = _tutorialProgressPrcKey.Invoke(),
				UserId = userId
			});

			string value = response?.Value;
			if (value == null)
				return Array.Empty<TutorialProgressPrcDto>();

			return JsonSerializer.Deserialize<TutorialProgressPrcDto[]>(value) ?? Array.Empty<TutorialProgressPrcDto>();
		}

		public async ValueTask<bool> Save(string userId, TutorialProgressPrcDto[] dtos)
		{
			CommonGrpcResponse commonGrpcResponse = await _serverKeyValueService.TryCall(service => service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = _tutorialProgressPrcKey.Invoke(),
						Value = JsonSerializer.Serialize(dtos)
					}
				}
			}));

			return commonGrpcResponse.IsSuccess;
		}
	}
}
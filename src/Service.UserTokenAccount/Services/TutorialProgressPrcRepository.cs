using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.Education.Structure;
using Service.Grpc;
using Service.ServerKeyValue.Grpc;
using Service.ServerKeyValue.Grpc.Models;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Services
{
	public interface ITutorialProgressPrcRepository
	{
		ValueTask<TutorialProgressPrcDto> Get(string userId, EducationTutorial tutorial);
		ValueTask<bool> Save(string userId, TutorialProgressPrcDto dto);
	}

	public class TutorialProgressPrcRepository : ITutorialProgressPrcRepository
	{
		private readonly Func<string> _tutorialProgressPrcKey = Program.ReloadedSettings(model => model.KeyTutorialProgressPrc);

		private readonly IGrpcServiceProxy<IServerKeyValueService> _serverKeyValueService;

		public TutorialProgressPrcRepository(IGrpcServiceProxy<IServerKeyValueService> serverKeyValueService) => _serverKeyValueService = serverKeyValueService;

		public async ValueTask<TutorialProgressPrcDto> Get(string userId, EducationTutorial tutorial)
		{
			ValueGrpcResponse response = await _serverKeyValueService.Service.GetSingle(new ItemsGetSingleGrpcRequest
			{
				Key = _tutorialProgressPrcKey.Invoke(),
				UserId = userId
			});

			string value = response?.Value;
			if (value != null)
			{
				TutorialProgressPrcDto[] info = JsonSerializer.Deserialize<TutorialProgressPrcDto[]>(value);

				TutorialProgressPrcDto dto = info?.FirstOrDefault(dto => dto.Tutorial == tutorial);

				if (dto != null)
					return dto;
			}

			return new TutorialProgressPrcDto {Tutorial = tutorial};
		}

		public async ValueTask<bool> Save(string userId, TutorialProgressPrcDto dto)
		{
			CommonGrpcResponse commonGrpcResponse = await _serverKeyValueService.TryCall(service => service.Put(new ItemsPutGrpcRequest
			{
				UserId = userId,
				Items = new[]
				{
					new KeyValueGrpcModel
					{
						Key = _tutorialProgressPrcKey.Invoke(),
						Value = JsonSerializer.Serialize(dto)
					}
				}
			}));

			return commonGrpcResponse.IsSuccess;
		}
	}
}
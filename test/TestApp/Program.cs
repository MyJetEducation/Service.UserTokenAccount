using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Grpc;
using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Client;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Grpc;
using Service.UserTokenAccount.Grpc.Models;
using GrpcClientFactory = ProtoBuf.Grpc.Client.GrpcClientFactory;

namespace TestApp
{
	public class Program
	{
		private static async Task Main()
		{
			GrpcClientFactory.AllowUnencryptedHttp2 = true;
			ILogger<Program> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

			Console.Write("Press enter to start");
			Console.ReadLine();

			var factory = new UserTokenAccountClientFactory("http://localhost:5001", logger);
			IGrpcServiceProxy<IUserTokenAccountService> serviceProxy = factory.GetUserTokenAccountService();
			IUserTokenAccountService client = serviceProxy.Service;

			var userId = Guid.NewGuid().ToString();

			//Check zero value at start
			AccountGrpcResponse getAccount1Response = await client.GetAccountAsync(new GetAccountGrpcRequest {UserId = userId});
			if (getAccount1Response == null)
				throw new Exception($"Can't get account (1) for user {userId}");
			decimal value0 = getAccount1Response.Value;
			if (value0 != 0m)
				throw new Exception($"Error account value ({value0}) for user {userId}");

			Console.WriteLine($"Account for user: {userId}");
			Console.WriteLine(JsonConvert.SerializeObject(getAccount1Response));

			//New token operation 1
			NewOperationGrpcResponse newOperation1Response = await client.NewOperationAsync(new NewOperationGrpcRequest
			{
				UserId = userId,
				Value = 100.5m,
				Info = "{\"info\": 1}",
				Movement = TokenOperationMovement.Income,
				Source = TokenOperationSource.TokenPurchase
			});
			if (newOperation1Response is not {Result: TokenOperationResult.Ok, Value: 100.5m })
				throw new Exception($"Can't save new operation (1) for user {userId}");

			Console.WriteLine($"Saved new (1) operation, response: {userId}");
			Console.WriteLine(JsonConvert.SerializeObject(newOperation1Response));

			//New token operation 2
			NewOperationGrpcResponse newOperation2Response = await client.NewOperationAsync(new NewOperationGrpcRequest
			{
				UserId = userId,
				Value = 50m,
				Info = "{\"info\": 2}",
				Movement = TokenOperationMovement.Income,
				Source = TokenOperationSource.TokenPurchase
			});
			if (newOperation2Response is not {Result: TokenOperationResult.Ok, Value: 150.5m })
				throw new Exception($"Can't save new operation (2) for user {userId}");

			Console.WriteLine($"Saved new (2) operation, response: {userId}");
			Console.WriteLine(JsonConvert.SerializeObject(newOperation2Response));

			//Check new value
			AccountGrpcResponse getAccount2Response = await client.GetAccountAsync(new GetAccountGrpcRequest {UserId = userId});
			if (getAccount2Response == null)
				throw new Exception($"Can't get account (2) for user {userId}");

			decimal value1 = getAccount2Response.Value;
			if (value1 != 150.5m)
				throw new Exception($"Error account value ({value1}) for user {userId}");

			//New product operation 1
			NewOperationGrpcResponse newOperation3Response = await client.NewOperationAsync(new NewOperationGrpcRequest
			{
				UserId = userId,
				Value = 150m,
				Info = "{\"info\": 3}",
				Movement = TokenOperationMovement.Outcome,
				ProductType = MarketProductType.MascotSkin,
				Source = TokenOperationSource.ProductPurchase
			});
			if (newOperation3Response is not {Result: TokenOperationResult.Ok, Value: 0.5m })
				throw new Exception($"Can't save new operation (3) for user {userId}");

			Console.WriteLine($"Saved new (3) operation, response: {userId}");
			Console.WriteLine(JsonConvert.SerializeObject(newOperation3Response));

			//Check new value
			AccountGrpcResponse getAccount3Response = await client.GetAccountAsync(new GetAccountGrpcRequest {UserId = userId});
			if (getAccount3Response == null)
				throw new Exception($"Can't get account (3) for user {userId}");

			decimal value2 = getAccount3Response.Value;
			if (value2 != 0.5m)
				throw new Exception($"Error account value ({value2}) for user {userId}");

			//New product operation 2
			NewOperationGrpcResponse newOperation4Response = await client.NewOperationAsync(new NewOperationGrpcRequest
			{
				UserId = userId,
				Value = 10m,
				Info = "{\"info\": 4}",
				Movement = TokenOperationMovement.Outcome,
				ProductType = MarketProductType.RetryPack1,
				Source = TokenOperationSource.ProductPurchase
			});
			if (newOperation4Response is not { Result: TokenOperationResult.InsufficientAccount })
				throw new Exception($"Error state for insufficient response for user {userId}");

			//Get all
			OperationsGrpcResponse operationsResponse = await client.GetOperationsAsync(new GetOperationsGrpcRequest
			{
				UserId = userId
			});

			OperationGrpcModel[] operations = operationsResponse?.Operations;
			if (operations == null || operationsResponse.Operations.Length != 3)
				throw new Exception($"Incorrect number of operations for user {userId}");

			Console.WriteLine($"Operations for user: {userId}");
			Console.WriteLine(JsonConvert.SerializeObject(operationsResponse));

			Console.WriteLine("End");
			Console.ReadLine();
		}
	}
}
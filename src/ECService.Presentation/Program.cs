using ECService.Application.Extensions;
using ECService.Infrastructure.Extensions;
using ECService.Presentation.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 接続文字列
var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
    ?? throw new InvalidOperationException("接続文字列 'LibraryDb' が設定されていません。");

// Controller
builder.Services.AddControllers();

// 各層のDI登録
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();
builder.Services.AddPresentation();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "データ管理サービス（管理者向け）",
        Version = "v1",
        Description = "ECサービスの管理者サービスの REST API",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestAPI Exercise v1");
    c.RoutePrefix = string.Empty;
    c.UseRequestInterceptor("(request) => { request.credentials = 'include'; return request; }");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
//石原:いらなそうなコメントアウト消してまとめました
// using ECService.Application.Authentications;
// using ECService.Application.Extensions;
using ECService.Infrastructure.Extensions;
// using ECService.Presentation.Extensions;
// using ECService.Presentation.Middlewares;
// using ECService.Presentation.ViewModels;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using ECService.Applications.Usecases.Imps;
using ECService.Applications.Usecases.Interfaces;
using ECService.Presentations.Adapters;

var builder = WebApplication.CreateBuilder(args);


// --- 設定値の読み込み ---
// 接続文字列(インフラ層へ渡す)
var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
    ?? throw new InvalidOperationException("接続文字列 'LibraryDb' が設定されていません。");

// JWT 設定(アプリケーション層へ渡す)
// var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
//     ?? throw new InvalidOperationException("JWT 設定 'Jwt' が設定されていません。");

// --- 認証(JWT Bearer)---
// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         // トークンの検証パラメータ(発行時の JwtSettings と一致するか検証する)
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = jwtSettings.Issuer,

//             ValidateAudience = true,
//             ValidAudience = jwtSettings.Audience,

//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

//             ValidateLifetime = true,             // 有効期限を検証する
//             ClockSkew = TimeSpan.Zero,           // 期限のずれ許容をゼロに(既定は5分)
//         };

//         // トークンは Authorization ヘッダではなく、HttpOnly Cookie から読む
//         options.Events = new JwtBearerEvents
//         {
//             OnMessageReceived = context =>
//             {
//                 // ログイン時にセットした Cookie(access_token)から JWT を取得する
//                 if (context.Request.Cookies.TryGetValue("access_token", out var token))
//                 {
//                     context.Token = token;
//                 }
//                 return Task.CompletedTask;
//             },

//             // 未認証(トークンが無い・無効)で保護されたリソースにアクセスした場合の応答
//             OnChallenge = async context =>
//             {
//                 // 既定の応答(ボディ空・WWW-Authenticate ヘッダ)を抑制する
//                 context.HandleResponse();

//                 // 他のエラーと同じ形式(error/message)で 401 を返す
//                 context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                 context.Response.ContentType = "application/json";

//                 var body = new ErrorResponse
//                 {
//                     Error = "Unauthorized",
//                     Message = "認証が必要です。ログインしてください。"
//                 };

//                 var json = System.Text.Json.JsonSerializer.Serialize(body, new System.Text.Json.JsonSerializerOptions
//                 {
//                     PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
//                 });

//                 await context.Response.WriteAsync(json);
//             }
//         };
//     });
// builder.Services.AddAuthorization();

// --- DI 登録 ---
// Controller を使用する
builder.Services.AddControllers();

// 各層の構成要素を登録する(これまで各層に作成した拡張メソッド)
// インフラストラクチャ層
builder.Services.AddInfrastructure(connectionString);
// // アプリケーション層
// builder.Services.AddApplication(jwtSettings);
// // プレゼンテーション層
// builder.Services.AddPresentation();


// Swagger(タイトル・XML コメント付き)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "📚 データ管理サービス（管理者向け）",
        Version = "v1",
        Description = "ECサービスの管理者サービスの REST API",
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});


var app = builder.Build();

// 例外ハンドリングミドルウェアの追加
// app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger UI を開くのは開発環境のみだけど、とりあえず開発環境かどうかのチェックを外して強制的に Swagger UI を開くようにする
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         // テキストの手順にあった「ルートURLで開く」
//         c.RoutePrefix = string.Empty; 
//     });
// }

// 一旦 if 文のチェックを外して強制的に動かす
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestAPI Exercise v1");
    c.RoutePrefix = string.Empty;
    c.UseRequestInterceptor("(request) => { request.credentials = 'include'; return request; }");
});


app.UseHttpsRedirection();

// 認証 → 認可 の順(MapControllers の前)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

builder.Services.AddScoped<IGetProductInfoUsecase, GetProductInfoUsecase>();
builder.Services.AddScoped<GetProductViewModelAdapter>();
// インフラストラクチャ層
builder.Services.AddInfrastructure(connectionString);

// アプリケーション層
builder.Services.AddScoped<IGetProductInfoUsecase, GetProductInfoUsecase>();

// プレゼンテーション層
builder.Services.AddScoped<GetProductViewModelAdapter>();
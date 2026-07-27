using ECService.Application.Authentications;
using ECService.Application.Extensions;
using ECService.Infrastructure.Extensions;
using ECService.Presentation.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.Reflection;
using System.Text;

var builder =
    WebApplication.CreateBuilder(args);

// データベースへ接続するための接続文字列を取得する
var connectionString =
    builder.Configuration.GetConnectionString(
        "ECServiceDB")
    ?? throw new InvalidOperationException(
        "接続文字列 'ECServiceDB' が設定されていません。");

// Azure Blob StorageのコンテナーSAS URLを取得する
var containerSasUrl =
    builder.Configuration[
        "AzureBlobStorage:ContainerSasUrl"]; //石原:変更 接続文字列ではなくコンテナーSAS URLを取得する

if (string.IsNullOrWhiteSpace(
        containerSasUrl))
{
    throw new InvalidOperationException(
        "設定 'AzureBlobStorage:ContainerSasUrl' が設定されていません。");
}

// JWT設定を取得し、認証処理へ渡す
var jwtSettings =
    builder.Configuration
        .GetSection("Jwt")
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JWT 設定 'Jwt' が設定されていません。");

// CORS設定
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            // フロントエンドからのAPIアクセスを許可する
            policy
                .WithOrigins(
                    "http://127.0.0.1:5245",
                    "http://localhost:5245")
                .AllowAnyHeader()
                .AllowAnyMethod()
                // Cookieの送受信を許可する
                .AllowCredentials();
        });
});

// JWT Bearer認証
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // 発行されたJWTが正しいものか検証する
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.SecretKey)),

                // JWTの有効期限を確認する
                ValidateLifetime = true,

                // 有効期限のずれを許可しない
                ClockSkew = TimeSpan.Zero,
            };

        options.Events =
            new JwtBearerEvents
            {
                // Authorizationヘッダーではなく、
                // HttpOnly CookieからJWTを取得する
                OnMessageReceived = context =>
                {
                    if (
                        context.Request.Cookies
                            .TryGetValue(
                                "access_token",
                                out var token))
                    {
                        context.Token = token;
                    }

                    return Task.CompletedTask;
                },

                // 未認証の場合にJSON形式で401を返す
                OnChallenge = async context =>
                {
                    // JWT Bearerの既定レスポンスを抑制する
                    context.HandleResponse();

                    context.Response.StatusCode =
                        StatusCodes
                            .Status401Unauthorized;

                    context.Response.ContentType =
                        "application/json";

                    var body =
                        new
                        {
                            message =
                                "認証が必要です。ログインしてください。",
                        };

                    var json =
                        System.Text.Json
                            .JsonSerializer
                            .Serialize(
                                body,
                                new System.Text.Json
                                    .JsonSerializerOptions
                                {
                                    PropertyNamingPolicy =
                                        System.Text.Json
                                            .JsonNamingPolicy
                                            .CamelCase,
                                });

                    await context.Response
                        .WriteAsync(json);
                },

                // 認証済みだが権限がない場合に403を返す
                OnForbidden = async context =>
                {
                    context.Response.StatusCode =
                        StatusCodes
                            .Status403Forbidden;

                    context.Response.ContentType =
                        "application/json";

                    var body =
                        new
                        {
                            message =
                                "アクセスが許可されていません。",
                        };

                    var json =
                        System.Text.Json
                            .JsonSerializer
                            .Serialize(
                                body,
                                new System.Text.Json
                                    .JsonSerializerOptions
                                {
                                    PropertyNamingPolicy =
                                        System.Text.Json
                                            .JsonNamingPolicy
                                            .CamelCase,
                                });

                    await context.Response
                        .WriteAsync(json);
                },
            };
    });

builder.Services.AddSingleton(
    jwtSettings);

builder.Services.AddAuthorization();

// Controller
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // ModelStateが不正な場合に自動で400を返さず、
        // Controller側でエラー内容を制御できるようにする
        options.SuppressModelStateInvalidFilter =
            true;
    });

// 各層のDI登録
builder.Services.AddInfrastructure(
    connectionString,
    containerSasUrl); //石原:変更 Blob接続用のコンテナーSAS URLを渡す

builder.Services.AddApplication();
builder.Services.AddPresentation();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.OpenApiInfo
        {
            Title =
                "データ管理サービス（管理者向け）",

            Version = "v1",

            Description =
                "ECサービスの管理者サービスの REST API",
        });

    var xmlFile =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath =
        Path.Combine(
            AppContext.BaseDirectory,
            xmlFile);

    options.IncludeXmlComments(
        xmlPath,
        includeControllerXmlComments: true);
});

var app =
    builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "RestAPI Exercise v1");

    options.RoutePrefix =
        string.Empty;

    // SwaggerからCookieを送信できるようにする
    options.UseRequestInterceptor(
        "(request) => { " +
        "request.credentials = 'include'; " +
        "return request; " +
        "}");
});

app.UseHttpsRedirection();

// フロントエンドからの通信へCORS設定を適用する
app.UseCors(
    "AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
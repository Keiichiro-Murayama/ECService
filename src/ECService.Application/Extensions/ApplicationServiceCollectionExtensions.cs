using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
//using ECService.Infrastructure.Contexts;
//using ECService.Domains.Models;
//using ECService.Infrastructure.Adapters;
//using ECService.Infrastructure.Repositories;
//using ECService.Domains.Repositories;
//using ECService.Infrastructure.Shared;
//using ECService.Application.Usecases;
//using ECService.Application.Usecases.Interactors;
//using ECService.Application.Usecases.Interfaces;
//using ECService.Presentation.Adapters;

using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.Imps;

using ECService.Application.Security;
using Microsoft.AspNetCore.Identity;
///using Microsoft.IdentityModel.Tokens;
using ECService.Application.Employees.Interactors;
using ECService.Application.Usecases.Employees.Interfaces;
///using ECService.Infrastructure.Security;
///using ECService.Application.Usecases.Authenticate.Interactors;
///using ECService.Application.Usecases.Authenticate.Interfaces;
using ECService.Domain.Models;
namespace ECService.Presentation.Configs;
/// <summary>
/// 依存関係(DI)の設定
/// インフラストラクチャ層、アプリケーション層、プレゼンテーション層
/// をまとめて追加する拡張クラス
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// アプリ全体の依存関係を一括追加する拡張メソッド
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <param name="config">構成情報</param>
    /// <returns>IServiceCollection(チェーン可能)</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        // インフラストラクチャ層の依存関係を追加
        services.AddInfrastructureDependencies(config);
        // アプリケーション層の依存関係を追加
        services.AddApplicationLayerDependencies(config);
        // プレゼンテーション層の依存関係を追加
        services.AddPresentationLayerDependencies(config);
        return services;
    }

    /// <summary>
    /// アプリケーション層の依存関係を追加
    /// </summary>
    /// <param name="services">依存関係注入(DI)のサービスコレクション</param>
    /// <param name="config"></param>
    /// <returns></returns>
    private static IServiceCollection AddApplicationLayerDependencies(
        this IServiceCollection services, IConfiguration config)
    {
        //ユースケース：[新商品を登録する]を実現するインターフェース
        //services.AddScoped<IRegisterBookUsecase, RegisterBookUsecase>();
        //ユースケース[商品を変更する]を実現するインターフェース
        //services.AddScoped<IUpdateBookUsecase, UpdateBookUsecase>();
        //ユースケース[商品をキーワード検索する]を実現するインターフェース
        //services.AddScoped<ISearchBookByKeywordUsecase, SearchBookByKeywordUsecase>();
        //ユースケース：[新商品を削除する]を実現するインターフェース
        // services.AddScoped<IDeleteBookUsecase, DeleteBookUsecase>();
        // UpdateProductViewModelからドメインオブジェクト:Productへ変換するアダプタ
        // ASP.NET Core Identityのパスワードハッシュ化・検証機能
        services.AddScoped<IPasswordHasher<EmployeeAccount>, PasswordHasher<EmployeeAccount>>();
        // PBKDF2アルゴリズムを利用したパスワードハッシュ化・検証機能
        services.AddScoped<IPasswordService, PasswordService>();
        // ユースケース:[ユーザーを登録する]を実現するインターフェイス
        services.AddScoped<IRegisterEmployeeAccountUsecase, RegisterEmployeeAccountUsecase>();
        // JwtSettingsをバインドしてDIに登録する
        //services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        // ユースケース:[ログインする]を実現するインターフェイス
        // services.AddScoped<IAuthenticateUserUsecase, AuthenticateUserUsecase>();





        // ユースケース:[商品を検索する]を実現するインターフェイス
        services.AddScoped<ISearchProductsUseCase, SearchProductsUseCase>();


        return services;
    }
}


/// <summary>
/// テストプロジェクトにServiceProviderを提供するヘルパメソッド
/// </summary>
/// <param name="config"></param>
/// <param name="configureServices"></param>
/// <param name="configureLogging"></param>
/// <returns></returns>
/* public static ServiceProvider BuildAppProvider(
    IConfiguration config,
    Action<IServiceCollection>? configureServices = null,
    Action<ILoggingBuilder>? configureLogging = null)
 {
     var services = new ServiceCollection();
     services.AddLogging(b =>
     {
         if (configureLogging is not null) configureLogging(b);
         else b.AddConsole().SetMinimumLevel(LogLevel.Warning);
     });
     services.AddApplicationDependencies(config);
     configureServices?.Invoke(services);

     return services.BuildServiceProvider(validateScopes: true);
 }
}*/
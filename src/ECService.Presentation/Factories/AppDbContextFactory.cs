using ECService.Infrastructure.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECService.Presentation.Factories;

/// <summary>
/// EF Core Tools実行時に
/// AppDbContextを生成するファクトリ
/// </summary>
public sealed class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// マイグレーション実行時に
    /// AppDbContextを生成する
    /// </summary>
    /// <param name="args">
    /// コマンドライン引数
    /// </param>
    /// <returns>
    /// AppDbContext
    /// </returns>
    public AppDbContext CreateDbContext(
        string[] args)
    {
        string environmentName =
            Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT")
            ?? "Production";

        string presentationDirectory =
            GetPresentationDirectory();

        IConfiguration configuration =
            new ConfigurationBuilder()
                .SetBasePath(
                    presentationDirectory)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: false)
                .AddJsonFile(
                    $"appsettings.{environmentName}.json",
                    optional: true,
                    reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

        string connectionString =
            configuration.GetConnectionString(
                "ECServiceDB")
            ?? throw new InvalidOperationException(
                "接続文字列 'ECServiceDB' が設定されていません。");

        var optionsBuilder =
            new DbContextOptionsBuilder<
                AppDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new AppDbContext(
            optionsBuilder.Options);
    }

    /// <summary>
    /// Presentationプロジェクトの
    /// ディレクトリを取得する
    /// </summary>
    /// <returns>
    /// Presentationプロジェクトのパス
    /// </returns>
    private static string
        GetPresentationDirectory()
    {
        string currentDirectory =
            Directory.GetCurrentDirectory();

        string presentationDirectory =
            Path.Combine(
                currentDirectory,
                "src",
                "ECService.Presentation");

        if (
            Directory.Exists(
                presentationDirectory))
        {
            return presentationDirectory;
        }

        /*
         * Presentationプロジェクト内から
         * dotnet efを実行した場合
         */
        if (
            File.Exists(
                Path.Combine(
                    currentDirectory,
                    "ECService.Presentation.csproj")))
        {
            return currentDirectory;
        }

        throw new DirectoryNotFoundException(
            "ECService.Presentationプロジェクトのディレクトリを取得できませんでした。");
    }
}
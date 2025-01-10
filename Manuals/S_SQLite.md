# SQLite

データベースのSQLiteを扱うためのクラス。

ファイルはexeと同じディレクトリに作成され、Entityクラスを使ってやり取りする。

### データベース作成設定

設定されていない場合、ApplicationHostServiceのStartAsyncにSQLiteFileManager.CreateDatabaseFile();を実装

```csharp
public class ApplicationHostService : IHostedService
{
    //アプリ起動時に実行されるコード
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Settinghelper.SettingLoad();
        SQLiteFileManager.CreateDatabaseFile();
        return Task.CompletedTask;
    }

    //アプリ終了時に実行されるコード
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

### Entityの作成

DomainのEntitiesフォルダにxxxEntityクラスを作成。

プライマリキーを設定

![image.png](assets/image%206.png)

```csharp
public sealed class UserEntity
{
    [PrimaryKey]
    public string UserId { get; }
    public string UserName { get; }

    public UserEntity(string userId, string userName)
    {
        UserId = userId;
        UserName = userName;
    }
}
```

※プロパティ作成後、インテリセンスから「コンストラクターを生成する…」を選択すると簡単に作成ができる

![image.png](assets/image%207.png)

![image.png](assets/image%208.png)

### SQLiteFileManager登録

SQLiteデータベースへテーブル登録するためのコードを実装

```csharp
public static void CreateDatabaseFile()
{
    if (!File.Exists(SQLiteHelper.DbPath))
    {
        SQLiteConnection.CreateFile(SQLiteHelper.DbPath);
    }
    //　テーブル作成
    CreateTable(typeof(UserEntity));
}
```

### 読み書き用クラス作成

InfrastructureのSQLiteフォルダにxxxSQLiteを作成

![image.png](assets/image%209.png)

SQLiteBaseの基底クラスを継承、初期値を設定

```csharp
public class UserSQLite : SQLiteBase<UserEntity>
{
    public UserSQLite() : base(() => new UserEntity(string.Empty, string.Empty))
    {
    }
}
```

### DI登録

```csharp
private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    //Service登録
    services.AddHostedService<ApplicationHostService>();
    services.AddSingleton<TransitionService>();
    services.AddSingleton<MessageService>();
    services.AddSingleton<DialogService>();

    //SQLite登録
    services.AddSingleton<UserSQLite>();

    //View / ViewModel登録
    //AddSingleton: 一度生成されたインスタンスを使いまわす
    //AddTransient: 使用するたびに新しいインスタンスを生成する
    services.AddTransient<MainWindow>();
    services.AddTransient<MainWindowViewModel>();
    services.AddTransient<HomeView>();
    services.AddTransient<HomeViewModel>();
}
```

### 呼び出し側

使用するクラスのコンストラクタで受け取り

```csharp
public HomeViewModel(UserSQLite userSQLite)
{
		_userSQLite = userSQLite;
}
```

### 使用方法

SQLiteBaseは簡易ORMの機能を持っており、関数を使って基本的なデータのやり取りが可能。

| メソッド名 | 説明 | 使用例 |
| --- | --- | --- |
| ClearTable | テーブルの内容をすべて削除 | userSQLite.ClearTable(); |
| DeleteSingle | 単一のデータを削除する | userSQLite.DeleteSingle(nameof(UserEntity.UserId), "123"); |
| DeleteList | 複数のデータを削除する | userSQLite.DeleteList(nameof(UserEntity.UserId), new List<object>{ "123","124", }); |
| GetAllTableData | テーブルの内容を全て取得 | var ret = userSQLite.GetAllTableData(); |
| GetDataSingle | 指定のプロパティで値で検索した値を取得 | var ret = userSQLite.GetDataSingle(nameof(UserEntity.UserId), "123"); |
| GetDataList | 指定のプロパティで値で検索した値をリストで取得 | var ret = userSQLite.GetDataList(nameof(UserEntity.UserName), "aaa"); |
| SetDataSingle | テーブルにデータを追加 | userSQLite.SetDataSingle(new UserEntity("123", "aaa")); |
| SetDataList | テーブルに複数データを追加 | userSQLite.SetDataList(new List<UserEntity>
{
new UserEntity("123","aaa"),
new UserEntity("456","bbb"),
}); |
| UpdateSingle | 単一のデータを更新する | userSQLite.UpdateSingle(nameof(UserEntity.UserId), new UserEntity("123", "aaa")); |
| UpdateList | 複数のデータを更新する | userSQLite.UpdateList(nameof(UserEntity.UserId),new List<UserEntity>
{
new UserEntity("123","ccc"),
new UserEntity("456","ddd"),
}); |
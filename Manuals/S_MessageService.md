# MessageService(メッセージ表示)

Snackbarやメッセージウインドウ、入力ダイアログなどの表示には、MessageServiceを使用します。

### MessageServiceの受け取り

使用したいViewModelのコンストラクタでMessageServiceを受け取り、プライベートフィールド変数で保持します。

```csharp
private readonly MessageService _message;
public LoginViewModel(MessageService message)
{
  _message = message;
}
```

### メソッドの実行

使用したい場面でMessageServiceに実装されている該当のメソッドを実行することで、画面に表示が現れます。

```csharp
_message.ShowSnackbar("ログインしました");
```

| メソッド名 | 機能 | 使用例 |
| --- | --- | --- |
| ShowSnackbar | スナックバーでメッセージ表示 | _message.ShowSnackbar("メッセージ"); |
| ShowMessageDialog | ダイアログでメッセージ表示 | _message.ShowMessageDialog("メッセージ"); |
| ShowCautionDialog | セカンダリ色ダイアログでメッセージ表示 | _message.ShowCautionDialog("メッセージ"); |
| ShowErrorDialog | セカンダリ色ダイアログでタイトルありメッセージ表示 | _message.ShowErrorDialog(”タイトル”, "メッセージ"); |
| ShowYesNoDialog | 「はい」「いいえ」のダイアログ表示 | _message.ShowYesNoDialog("メッセージ"); |
| ShowYesCancelDialog | 「はい」「キャンセル」のダイアログ表示 | _message.ShowYesCancelDialog("メッセージ"); |
| ShowInputDialog | 入力ダイアログ表示 | _message.ShowInputDialog("メッセージ"); |
| ShowPasswordDialog | パスワード入力ダイアログ表示 | _message.ShowPasswordDialog("メッセージ"); |
| ShowLoginDialog | ログインダイアログ表示 | _message.ShowLoginDialog("メッセージ"); |
| ShowProcessingDialogAsync | 処理中ダイアログ表示 | await _message.ShowProcessingDialogAsync("メッセージ", () =>
{
// 時間のかかる処理
}); |
| ShowProcessingDialogBeforeAsync | 処理中ダイアログ表示 | await _message.ShowProcessingDialogBeforeAsync("メッセージ", () =>
{
// 時間のかかる処理
},
() =>
{
// 事前処理
}
); |
| ShowProcessingDialogAfterAsync | 処理中ダイアログ表示 | await _message.ShowProcessingDialogAfterAsync("メッセージ", () =>
{
// 時間のかかる処理
},
() =>
{
//　事後処理
}
); |
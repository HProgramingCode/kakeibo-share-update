namespace KakeiboShare.Domain.Settlements;

/// <summary>送金1本。From が To に Amount 円を支払う。</summary>
public readonly record struct Transfer(Guid From, Guid To, int Amount);

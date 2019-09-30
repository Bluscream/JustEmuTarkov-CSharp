using System;
using System.Runtime.InteropServices;
internal class DiscordRpc {
[DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Initialize", CharSet = CharSet.Unicode)]
public static extern void Initialize(string applicationId, ref DiscordRpc.EventHandlers handlers, bool autoRegister, string optionalSteamId);
[DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Shutdown")]
public static extern void Shutdown();
[DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_RunCallbacks")]
public static extern void RunCallbacks();
[DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_UpdatePresence")]
public static extern void UpdatePresence(ref DiscordRpc.RichPresence presence);
[DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_ClearPresence")]
public static extern void ClearPresence();
[DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Respond", CharSet = CharSet.Unicode)]
public static extern void Respond(string userId, DiscordRpc.Reply reply);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ReadyCallback();
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void DisconnectedCallback(int errorCode, string message);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ErrorCallback(int errorCode, string message);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void JoinCallback(string secret);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void SpectateCallback(string secret);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void RequestCallback(ref DiscordRpc.JoinRequest request);
public struct EventHandlers {
	public ReadyCallback readyCallback;
	public DisconnectedCallback disconnectedCallback;
	public ErrorCallback errorCallback;
	public JoinCallback joinCallback;
	public SpectateCallback spectateCallback;
	public RequestCallback requestCallback;
}
[Serializable]
public struct RichPresence
{
	public string state;
	public string details;
	public long startTimestamp;
	public long endTimestamp;
	public string largeImageKey;
	public string largeImageText;
	public string smallImageKey;
	public string smallImageText;
	public string partyId;
	public int partySize;
	public int partyMax;
	public string matchSecret;
	public string joinSecret;
	public string spectateSecret;
	public bool instance;
}
[Serializable]
public struct JoinRequest
{
	public string userId;
	public string username;
	public string discriminator;
	public string avatar;
}
public enum Reply
{
	No,
	Yes,
	Ignore
}
}
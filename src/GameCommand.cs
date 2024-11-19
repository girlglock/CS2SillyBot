/*
Copyright (C) 2024 Deana Brcka

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#pragma warning disable CA1416

using System.Net.Sockets;
using System.Text;
using WindowsInput;

public class TelnetClient : IDisposable
{
    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private bool _disposed = false;

    public TelnetClient(string hostname, int port)
    {
        _tcpClient = new TcpClient(hostname, port);
        _networkStream = _tcpClient.GetStream();
    }

    public async Task<string> SendCommandAsync(string command)
    {
        if (_tcpClient == null || !_tcpClient.Connected)
        {
            throw new InvalidOperationException("Not connected to the Telnet server.");
        }

        byte[] commandBytes = Encoding.ASCII.GetBytes(command + "\r\n");
        await _networkStream!.WriteAsync(commandBytes);

        byte[] buffer = new byte[1024];
        StringBuilder response = new();
        int bytesRead = 0;

        do
        {
            bytesRead = await _networkStream.ReadAsync(buffer);
            response.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
        } while (_networkStream.DataAvailable);

        return response.ToString();
    }

    public void Close()
    {
        Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_networkStream != null)
            {
                _networkStream.Close();
                _networkStream = null;
            }
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~TelnetClient()
    {
        Dispose(false);
    }
}

partial class CS2SillyBot
{
    public static async Task SendExecCommand(string msg, int delay = 200)
    {
        await semaphore.WaitAsync();
        try
        {
            using StreamWriter writer = new(gamePath! + "cfg/CS2SillyCommand.cfg", false);
            await writer.WriteLineAsync(msg);
            await Task.Delay(delay);
            _ = Task.Run(() => new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.DIVIDE));
        }
        catch (Exception ex)
        {
            Console.WriteLine("error writing file: " + ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public static TelnetClient? telnetClient = null;

    public static async Task SendCommand(string command)
    {
        await semaphore.WaitAsync();
        try
        {
            telnetClient ??= new(hostname, port);
            await telnetClient.SendCommandAsync(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("error using netcon: " + ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
    }
}

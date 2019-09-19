using System;
using System.IO.Ports;
using System.Threading;
using System.Web.Http;

namespace BalancaApiService
{
    public class BalancaController : ApiController
    {
        private int _timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeout"].ToString());

        // GET api/balanca
        public string Get(string portName, int baudRate, int parity, int dataBits, int stopBits)
        {
            try
            {
                using (var _serial = new SerialPort(portName, baudRate, (Parity)parity, dataBits, stopBits == 0 ? (StopBits)1 : (StopBits)stopBits ))
                {
                    // Verifica se a porta já esta aberta senão abre.
                    if (!_serial.IsOpen)
                        _serial.Open();

                    // Contador que aguarda que tenha buffer na serial
                    // caso não tenha tenta ler por 60 segundos e sai da rotina com um erro.
                    int contador = 0;
                    while (_serial.BytesToRead <= 0 && contador < _timeout)
                    {
                        Thread.Sleep(1000);
                        contador++;
                    }
                    if (contador == _timeout)
                        return "Timeout - Não foi possível ler o peso na balança.";

                    //aguarda termino do buffer para garantir que a balanca enviou tudo.
                    contador = 0;
                    int intBytes = _serial.BytesToRead;
                    while (contador < 4)
                    {
                        intBytes = _serial.BytesToRead;
                        Thread.Sleep(250);
                        contador++;
                        if (contador == 4 && _serial.BytesToRead == intBytes)
                            break;
                    }


                    // Quantidade de bytes no buffer da serial
                    intBytes = _serial.BytesToRead;

                    // Cria buffer de leitura
                    byte[] bytes = new byte[intBytes];

                    // Lê dados na porta
                    _serial.Read(bytes, 0, intBytes);

                    // Formata a leitura removendo caracteres não visuais
                    for (int i = 0; i < intBytes; i++)
                    {
                        char ch = Convert.ToChar(bytes[i]);
                        if (char.IsControl(ch) && bytes[i].ToString() != " ")
                            bytes[i] = Convert.ToByte('.');
                    }

                    // Retorna a leitura
                    return System.Text.Encoding.Default.GetString(bytes);
                }
            }
            catch (TimeoutException tex)
            {
                return tex.Message;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
    }
}

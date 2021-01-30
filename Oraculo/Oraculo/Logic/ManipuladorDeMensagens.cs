using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Oraculo.Logic
{
    class ManipuladorDeMensagens : IManipuladorDeMensagens, IDisposable
    {
        public event MensagemRecebidaDelegate MensagemRecebida;
        private readonly IDatabase _db;
        private readonly ISubscriber _pub;
        private readonly ISubscriber _sub;
        private const string CHANNEL = "perguntas";
        private const string HASHID = "HASH_ID";
        private const string NOME_GRUPO = "GOOGLE_ORACULO";
        private Dictionary<string, string> _redisKeys;

        public ManipuladorDeMensagens(string connectionString)
        {
            var client = ConnectionMultiplexer.Connect(connectionString);
            this._db = client.GetDatabase();
            this._pub = client.GetSubscriber();
            this._sub = _pub;
            _redisKeys = new Dictionary<string, string>();

            IniciaColetorDeMensagens();
        }

        public void EnviaMensagem(Mensagem mensagem)
        {
            EnviaHash(mensagem);
        }

        private void IniciaColetorDeMensagens()
        {
            _pub.Subscribe(CHANNEL, (ch, msg) =>
            {
                string mensagem = msg.ToString();
                
                if (MensagemRecebida != null && mensagem.StartsWith("P"))
                {
                    if (mensagem.Contains(":"))
                    {
                        var msgRec = mensagem.Split(':');

                        MensagemRecebida(new Mensagem() { Id = msgRec[0], Texto = msgRec[1] });
                    }
                }
            });
        }

        private void EnviaHash(Mensagem mensagem)
        {
            var Id = mensagem.Id;
            var texto = mensagem.Texto;

            _db.HashSet(Id, new HashEntry[] { new HashEntry(NOME_GRUPO, texto) });

            if (!_redisKeys.ContainsKey(Id))
            {
                _redisKeys.Add(Id, NOME_GRUPO);
            }
            else
            {
                _redisKeys[Id] = NOME_GRUPO;
            }
        }

        private void ClearHash(string redisKey, string redisValue)
        {
            if (_db.HashExists(redisKey, redisValue))
                _db.HashDelete(redisKey, redisValue);
        }

        public void Dispose()
        {
            if (_redisKeys.Count() > 0)
            {
                foreach (KeyValuePair<string,string> pair in _redisKeys)
                {
                    ClearHash(pair.Key, pair.Value);
                }
            }
        }
    }
}

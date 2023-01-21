using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaPomodoro.data
{
    public class Tarefa
    {
        public string TipoCiclo { get; set; }
        public string Nome { get; set; }
        public string Inicio { get; set; }
        public string Fim { get; set; }
        public string Duracao { get; set; }
    }
}

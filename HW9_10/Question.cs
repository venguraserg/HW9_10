using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW9_10
{
    public class Question
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string[] Answers { get; set; }

        public int CorrectAnswer { get; set; }

    }
}

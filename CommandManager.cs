using Space_RPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class CommandManager
    {
        private enum CommandType
        {
            Unknown,
            Assignment, // assign xx to/as xx
            Hiring,     // hire new crew
            Hire,       // hire
        }
        #region Public Methods
        public void ProcessCommand(string command)
        {
            // Get the type of command
            if (!GetType(command, out CommandType type, out string err))
            {
                MessageBox.Show("Error Command: " + err);
                return;
            }

            switch (type)
            {
                case CommandType.Assignment:

                    break;

                case CommandType.Hiring:
                    MainWindow.Crew.FindApplicant();

                    break;

                case CommandType.Hire:

                    break;
            }

        }
        #endregion Public Methods

        #region Private Methods
        private bool GetType(string command, out CommandType type, out string err)
        {
            type = CommandType.Unknown;
            err = string.Empty;

            var action = command.Split(' ')[0].ToUpper();

            switch (action)
            {
                case "ASSIGN":
                    type = CommandType.Assignment;
                    break;

                case "HIRE":
                    if (command.Contains("new"))
                    {
                        type = CommandType.Hiring;
                    }
                    else
                    {
                        type = CommandType.Hire;
                    }
                    break;

                default:
                    err = "Unknown Command";
                    return false;
            }
            return true;
        }
        #endregion Private Methods
    }
}

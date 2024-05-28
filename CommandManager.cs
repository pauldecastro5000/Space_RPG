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
            string err = "";
            // Get the type of command
            if (!GetType(command, out CommandType type, out err))
            {
                MessageBox.Show("Error Command: " + err);
                return;
            }

            string name = "";
            string assignedTo = "";
            
            switch (type)
            {
                case CommandType.Assignment:
                    GetAssignment(command, out name, out assignedTo, out err);
                    MainWindow.mainVm.MyShip.Assign(name, assignedTo);
                    break;

                case CommandType.Hiring:
                    MainWindow.Crew.FindApplicant();
                    MainWindow.Crew.DisplayApplicant();
                    break;

                case CommandType.Hire:
                    name = command.Split(' ').Last();
                    MainWindow.Crew.HireApplicant(name);
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
        private bool GetAssignment(string command, out string name, out string assignedTo, out string err)
        {
            // assign xx to/as xx
            name = "";
            assignedTo = "";
            err = "Wrong format command";
            var field = command.Split(' ');
            if (field.Length >= 4)
            {
                name = field[1];
                assignedTo = field[3];
                return true;
            }
            else
                return false;
        }
        #endregion Private Methods
    }
}

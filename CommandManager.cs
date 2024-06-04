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
            CrewTask
                // Start the engine
                // Shutoff the engine
                // Take off
        }
        #region Public Methods
        public bool ProcessCommand(string command, out string err)
        {
            err = "";
            string name = "";
            string assignedTo = "";

            // Get the type of command
            GetType(command, out CommandType type);

            switch (type)
            {
                case CommandType.Assignment:
                    if (!GetAssignment(command, out name, out assignedTo, out err))
                        return false;

                    if (!MainWindow.mainVm.MyShip.Assign(name, assignedTo, out err))
                        return false;
                    break;

                case CommandType.Hiring:
                    if (MainWindow.mainVm.MyShip.State != Ship.state.Docked)
                    {
                        err = "There is no applicant in space...";
                        return false;
                    }
                    MainWindow.Crew.FindApplicant();
                    MainWindow.Crew.DisplayApplicant();
                    break;

                case CommandType.Hire:
                    if (MainWindow.mainVm.MyShip.State != Ship.state.Docked)
                    {
                        err = "There is no applicant in space...";
                        return false;
                    }
                    name = command.Split(' ').Last();
                    if (!MainWindow.Crew.GetApplicant(name, out Crew crew, out err))
                        return false;

                    if (!MainWindow.mainVm.MyShip.HireApplicant(crew, out err))
                        return false;

                    MainWindow.Crew.RemoveApplicant(crew);
                    break;
                case CommandType.CrewTask:
                    if (!ProcessCrewTask(command, out Crew.CrewJob Job, out err))
                        return false;
                    MainWindow.mainVm.MyShip.AddCrewTask(Job, command);
                    break;
            }
            return true;
        }
        #endregion Public Methods

        #region Private Methods
        private bool ProcessCrewTask(string command, out Crew.CrewJob job, out string err)
        {
            err = "";
            job = Crew.CrewJob.None;
            var cmd = command.ToUpper();
            if (cmd.Contains("ENGINE"))
            {
                if (cmd.Contains("START") || cmd.Contains("SHUTOFF"))
                {
                    job = Crew.CrewJob.Pilot;
                    return true;
                } else if (cmd.Contains("FIX"))
                {
                    job = Crew.CrewJob.Engineer;
                    return true;
                }
            }

            if (cmd.Contains("PREPARE") && cmd.Contains("LIFTOFF"))
            {
                job = Crew.CrewJob.All;
                return true;
            }

            err = $"Unknown command: {command}";
            return false;
        }
        private void GetType(string command, out CommandType type)
        {
            var cmd = command.ToUpper();
            type = CommandType.Unknown;


            // ASSIGNMENT
            if (cmd.Contains("ASSIGN"))
            {
                type = CommandType.Assignment;
                return;
            }
            // HIRING
            if (cmd.Contains("HIRE"))
            {
                if (command.Contains("new"))
                {
                    type = CommandType.Hiring;
                }
                else
                {
                    type = CommandType.Hire;
                }
                return;
            }
            //// CREW TASKS
            //if (cmd.Contains("ENGINE"))
            //{
            //    type = CommandType.CrewTask;
            //    return;
            //}
            type = CommandType.CrewTask;
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

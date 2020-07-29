using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;


namespace CoreCommon.VSExtensionLib.Components
{
    public class VsSolution
    {
        DTE2 _dteObj;

        public VsSolution()
        {

        }

        #region " DTE - Solution"

        public void CreateDte()
        {
            Type t = Type.GetTypeFromProgID("VisualStudio.DTE.16.0", true);
            _dteObj = (EnvDTE80.DTE2)Activator.CreateInstance(t, true);

            // Register the IOleMessageFilter to handle any threading 
            // errors.
            MessageFilter.Register();
        }

        public void SetDTE(DTE2 dte)
        {
            _dteObj = dte;
        }

        public Solution GetSolution()
        {
            return _dteObj?.Solution;
        }

        public void OpenSolution(string name)
        {
            GetSolution().Open(name);
        }

        public void CloseSolution()
        {
            _dteObj.Quit();
        } 

        #endregion

        public void GetProjects()
        {
            foreach (Project project in GetSolution().Projects)
            {
                Console.WriteLine(project.Name);
            }
        }
    }
}

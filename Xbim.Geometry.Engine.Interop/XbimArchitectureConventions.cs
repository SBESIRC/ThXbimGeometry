﻿using System;

namespace Xbim.Geometry.Engine.Interop
{
    /// <summary>
    /// A class representing the conventions we use for processor specific Geometry Engine library
    /// </summary>
    internal class XbimArchitectureConventions
    {
        public XbimArchitectureConventions()
        {
            if (Is64BitProcess())
            {
                Suffix = "64";
                SubFolder = "x64";
            }
            else
            {
                Suffix = "32";
                SubFolder = "x86";
            }
        }

        public string AssemblyName
        {
            get
            {
                return ModuleName + Suffix; 
            }
        }
            

        /// <summary>
        /// The suffix we apply to platform-specific assemblys in the current process architecture
        /// </summary>
        public string Suffix { get; private set; }
        /// <summary>
        /// The default subfolder to look for platform-specific assemblys in the current process architecture
        /// </summary>
        public string SubFolder { get; private set; }

        /// <summary>
        /// name of the dll that that holds the geometry functionality
        /// </summary>
        public string ModuleName
        {
            get
            {
                return "Xbim.Geometry.Engine2017";
                //#if VS2017
                //                return "Xbim.Geometry.Engine2017";
                //#else
                //                return "Xbim.Geometry.Engine";
                //#endif
            }
        }

        public static bool Is64BitProcess()
        {
            return (IntPtr.Size == 8);
        }
    }
}

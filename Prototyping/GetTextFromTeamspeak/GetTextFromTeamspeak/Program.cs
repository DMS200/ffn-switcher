﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace GetTextFromTeamspeak
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg,
            int wParam,
            StringBuilder lParam);

        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;
        private const int WM_CLEAR = 0x0303;

        private static string GetText(IntPtr hwnd)
        {
            int lngLength;
            StringBuilder strBuffer = new StringBuilder();
            int lngRet;

            lngLength = SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero) + 1;

            strBuffer.Capacity = lngLength;

            lngRet = SendMessage(hwnd, WM_GETTEXT, strBuffer.Capacity, strBuffer);
            if (lngRet > 0)
                return strBuffer.ToString().Substring(0, lngRet);
            return
                null;
        }


        private static string SetText(IntPtr hwnd)
        {
            int lngLength;
            StringBuilder strBuffer = new StringBuilder();
            int lngRet;

            //lngLength = SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero) + 1;
            //strBuffer.Capacity = lngLength;
            lngRet = SendMessage(hwnd, WM_CLEAR, 0, new StringBuilder(""));
            //if (lngRet > 0)
            //    return strBuffer.ToString().Substring(0, lngRet);
            //return
            //    null;
            return null;
        }


        #region GetLastLinesFromTeamspeak

        private static String LastLine = null;

        public static List<String> GetLastLinesFromTeamSpeak()
        {
            List<String> Output = new List<string>();
            String RawText = "";

                try
                {
                    IntPtr windowHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "TMainForm", "TeamSpeak 2");
                    IntPtr childHandle;

                    IntPtr PanelHandle = FindWindowEx(windowHandle, IntPtr.Zero, "TPanel", IntPtr.Zero);
                    IntPtr PanelHandle2 = FindWindowEx(windowHandle, PanelHandle, "TPanel", IntPtr.Zero);
                    //try to get a handle to IE's toolbar container
                    childHandle = FindWindowEx(PanelHandle2, IntPtr.Zero, "TRichEditWithLinks", IntPtr.Zero);
                    if (childHandle != IntPtr.Zero)
                    {
                        RawText = GetText(childHandle);
                    }
                #region Extract each line from the Teamspeak 2 Textwindow contents
                String[] splitter = new String[1];
                String[] RawTextLines = null;

                splitter[0] = "\r\n";

                if (RawText != "")
                {
                    RawTextLines = RawText.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                }

                #endregion

                for (int i = RawTextLines.Length - 1; i >= 0; i--)
                {
                    if (RawTextLines[i] == LastLine)
                    {
                        break;
                    }
                    else
                    {
                        Output.Insert(0,RawTextLines[i]);
                    }
                }

                #region SetLastLine
                if (Output.Count > 0)
                {
                    LastLine = Output[Output.Count - 1];
                }
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine("[FEHLER@TS] " + e.Message);
            }

            return Output;// strUrlToReturn;
        }

        public static List<String> ClearTeamspeakWindow()
        {
            List<String> Output = new List<string>();
            String RawText = "";

            try
            {
                IntPtr windowHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "TMainForm", "TeamSpeak 2");
                IntPtr childHandle;

                IntPtr PanelHandle = FindWindowEx(windowHandle, IntPtr.Zero, "TPanel", IntPtr.Zero);
                IntPtr PanelHandle2 = FindWindowEx(windowHandle, PanelHandle, "TPanel", IntPtr.Zero);
                //try to get a handle to IE's toolbar container
                childHandle = FindWindowEx(PanelHandle2, IntPtr.Zero, "TRichEditWithLinks", IntPtr.Zero);
                if (childHandle != IntPtr.Zero)
                {
                    RawText = SetText(childHandle);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("[FEHLER@TS] " + e.Message);
            }

            return Output;// strUrlToReturn;
        }

        #endregion

        static void Main(string[] args)
        {
            List<String> Lines;

            ClearTeamspeakWindow();

            /*while (true)
            {
                Lines = GetLastLinesFromTeamSpeak();

                foreach(String Line in Lines)
                {
                    Console.WriteLine(Line);
                }
                Thread.Sleep(100);
            }*/

            return;// strUrlToReturn;
        }
    }
}

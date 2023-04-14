using System.Diagnostics;

namespace Padoru.Diagnostics
{
    internal class StackTraceUtils
    {
        private static StackTrace currentStackTrace;
        private static StackFrame[] stackFrames;
        private static int cachedFrame = -1;

        public static int CurrentFrame { get; set; }
        public static StackTrace CurrentStackTrace => currentStackTrace;
        public static string ClassName
        {
            get
            {
                var frame = GetStackFrame();
                if (frame == null)
                {
                    return string.Empty;
                }
                
                return frame.GetMethod().DeclaringType.Name;
            }
        }
        public static string MethodName
        {
            get
            {
                var frame = GetStackFrame();
                if (frame == null)
                {
                    return string.Empty;
                }

                return frame.GetMethod().Name;
            }
        }

        public static void CacheStackTrace()
        {
            if (CurrentFrame != cachedFrame)
            {
                // Invalidate cache
                currentStackTrace = new StackTrace(true);
                stackFrames = currentStackTrace.GetFrames();
                cachedFrame = CurrentFrame;
            }
        }
        
        // Find the position of Debug.Log in the callstack 
        // and point to the next immediate element on the stack, which is the one we are interested in
        private static StackFrame GetStackFrame()
        {
            // TODO: Figure out why this happens in iOS and WebGL and fix it
            if (stackFrames.Length <= 0)
            {
                return null;
            }
            
            int debugCallPos = 0;
            foreach (var f in stackFrames)
            {
                var clss = f.GetMethod().DeclaringType;
                var methodName = f.GetMethod().Name;
                if (clss == typeof(Debug) && methodName.StartsWith("Log"))
                {
                    break;
                }
                debugCallPos++;
            }
            
            return stackFrames[debugCallPos + 1];
        }
    }
}
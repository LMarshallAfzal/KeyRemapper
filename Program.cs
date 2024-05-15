using System;
using System.Runtime.InteropServices;

class Program {

    [StructLayout(LayoutKind.Sequential)]
    struct input_event
    {
        public long time;
        public ushort type;
        public ushort code;
        public uint value;
    }

    [DllImport("libevdev.so.2")]
    // Initialise a new libevdev device.
    static extern IntPtr libevdev_new();

    [DllImport("libevdev.so.2")]
    // Grab or ungrab the device through a kernal EVIOCGRAB.
    static extern int libevdev_grab(IntPtr dev, int grab);

    [DllImport("libevdev.so.2")]
    // Set the fd for this struct and initialise internal data.
    static extern int libevdev_set_fd(IntPtr dev, int fd);

    [DllImport("libevdev.so.2")]
    // Get the next event from the device
    static extern int libevdev_next_event(IntPtr dev, uint flags, ref IntPtr ev);

    [DllImport("libevdev.so.2")]
    // Create a uinput device based on the given libevbdev device.
    static extern int libevdev_uinput_create_from_device(IntPtr dev, int uinput_fd, ref IntPtr uinput_dev);

    [DllImport("libevdev.so.2")]
    // Return the device node representing this uinput device.
    static extern void libevdev_uinput_destroy(IntPtr uinput_dev);

    [DllImport("libevdev.so.2")]
    // Post an event through the uinput device.
    static extern void libevdev_uinput_write_event(IntPtr uinput_dev, uint type, uint code, int value);

    [DllImport("libevdev.so.2")]
    static extern void libevdev_set_log_function(libevdev_log_func_t logfunc, IntPtr data);

    [DllImport("libevdev.so.2")]
    static extern void libevdev_set_log_priority(libevdev_log_priority priority);
    delegate void libevdev_log_func_t(IntPtr data, libevdev_log_priority priority, string msg);

    enum libevdev_log_priority
    {
        LIBEVDEV_LOG_ERROR = 10,
        LIBEVDEV_LOG_INFO = 20,
        LIBEVDEV_LOG_DEBUG = 30
    }

    static void LogCallback(IntPtr data, libevdev_log_priority priority, string msg)
    {
        Console.WriteLine($"[{priority}] {msg}");
    }

    const int EV_KEY = 0x01;
    const int KEY_A = 30;
    const int KEY_B = 48;
    const int KEY_ESC = 1;

    const uint LIBEVDEV_READ_FLAG_NORMAL = 0x00;
    
    static void Main(string[] args) 
    {
        // Set up logging
        libevdev_set_log_function(LogCallback, IntPtr.Zero);
        libevdev_set_log_priority(libevdev_log_priority.LIBEVDEV_LOG_ERROR);

        IntPtr dev = libevdev_new();
        IntPtr uinput_dev = IntPtr.Zero;

        // Open the keyboard device
        string devicePath = "/dev/input/event17";
        FileStream keyboardDeviceFs = File.Open(devicePath, FileMode.Open, FileAccess.Read);
        int fd = keyboardDeviceFs.SafeFileHandle.DangerousGetHandle().ToInt32();  // Replace 'X' with the appropriate device number
        Console.WriteLine($"File descriptor (fd): {fd}");

        int rc = libevdev_set_fd(dev, fd);
        Console.WriteLine($"Return code from libevdev_set_fd: {rc}\n");
        if (rc < 0)
        {
            Console.WriteLine($"Failed to set file descriptor: {rc}");
            Console.WriteLine($"\n");
            return;
        }

        // Create a virtual input device
        string virtualDevicePath = "/dev/uinput";
        FileStream virtualInputFs = File.Open(virtualDevicePath, FileMode.Open, FileAccess.Read);
        int uinput_fd = virtualInputFs.SafeFileHandle.DangerousGetHandle().ToInt32();
        Console.WriteLine($"Uinput File descriptor (uinput_fd): {uinput_fd}");

        rc = libevdev_uinput_create_from_device(dev, uinput_fd, ref uinput_dev);
        Console.WriteLine($"Return code from libevdev_uinput_create_from_device: {rc}\n");

        // Grab the device to prevent other applications from receiving events
        libevdev_grab(dev, 1);

        IntPtr ev = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(input_event)));
        while (true)
        {
            Console.WriteLine("Waiting for event...");
            Console.WriteLine($"dev: {dev}");
            Console.WriteLine($"LIBEVDEV_READ_FLAG_NORMAL: {LIBEVDEV_READ_FLAG_NORMAL}");
            Console.WriteLine($"ev: {ev}");

            rc = libevdev_next_event(dev, LIBEVDEV_READ_FLAG_NORMAL, ref ev);
            Console.WriteLine($"Return Code: {rc}\n");
            
            if (rc == 0)
            {
                int type = Marshal.ReadInt16(ev, 0);
                int code = Marshal.ReadInt16(ev, 2);
                int value = Marshal.ReadInt32(ev, 4);
                Console.WriteLine($"Event - Type: {type}, Code: {code}, Value: {value}\n");

                if (type == EV_KEY && code == KEY_A)
                {
                    // Remap 'A' to 'B'
                    code = KEY_B;
                }

                // Write the event to the virtual device
                Marshal.WriteInt16(ev, 2, (short)code);
                libevdev_uinput_write_event(uinput_dev, (uint)type, (uint)code, value);

                if (type == EV_KEY && code == KEY_ESC)
                {
                    break;
                }
            }
            if (rc < 0)
            {
                // An error occurred, but we don't have a specific error message
                Console.WriteLine($"Error: Failed to get the next event (return code: {rc})");
                break;
            }
        }

        // Clean up
        libevdev_grab(dev, 0);
        libevdev_uinput_destroy(uinput_dev);
        Marshal.FreeHGlobal(ev);
    }
}
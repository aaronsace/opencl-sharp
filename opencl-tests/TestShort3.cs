using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using OpenCl.Compiler;

namespace OpenCl.Tests
{
    [TestFixture]
    public class TestShort3
    {
        [Kernel]
        private static void test_short3_add([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] + b[i];
        }

        [Test]
        public void TestAdd()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_add,
                a,
                b,
                r
            );
            Assert.AreEqual(  12, r[0].s0);
            Assert.AreEqual(  24, r[0].s1);
            Assert.AreEqual(  36, r[0].s2);
            Assert.AreEqual(  12, r[1].s0);
            Assert.AreEqual(  24, r[1].s1);
            Assert.AreEqual(  36, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_add");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_add");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(  12, r[0].s0);
            Assert.AreEqual(  24, r[0].s1);
            Assert.AreEqual(  36, r[0].s2);
            Assert.AreEqual(  12, r[1].s0);
            Assert.AreEqual(  24, r[1].s1);
            Assert.AreEqual(  36, r[1].s2);
        }

        [Kernel]
        private static void test_short3_sub([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] - b[i];
        }

        [Test]
        public void TestSub()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_sub,
                a,
                b,
                r
            );
            Assert.AreEqual(   2, r[0].s0);
            Assert.AreEqual(   4, r[0].s1);
            Assert.AreEqual(   6, r[0].s2);
            Assert.AreEqual(  -2, r[1].s0);
            Assert.AreEqual(  -4, r[1].s1);
            Assert.AreEqual(  -6, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_sub");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_sub");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(   2, r[0].s0);
            Assert.AreEqual(   4, r[0].s1);
            Assert.AreEqual(   6, r[0].s2);
            Assert.AreEqual(  -2, r[1].s0);
            Assert.AreEqual(  -4, r[1].s1);
            Assert.AreEqual(  -6, r[1].s2);
        }

        [Kernel]
        private static void test_short3_mul([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] * b[i];
        }

        [Test]
        public void TestMul()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_mul,
                a,
                b,
                r
            );
            Assert.AreEqual(  35, r[0].s0);
            Assert.AreEqual( 140, r[0].s1);
            Assert.AreEqual( 315, r[0].s2);
            Assert.AreEqual(  35, r[1].s0);
            Assert.AreEqual( 140, r[1].s1);
            Assert.AreEqual( 315, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_mul");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_mul");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(  35, r[0].s0);
            Assert.AreEqual( 140, r[0].s1);
            Assert.AreEqual( 315, r[0].s2);
            Assert.AreEqual(  35, r[1].s0);
            Assert.AreEqual( 140, r[1].s1);
            Assert.AreEqual( 315, r[1].s2);
        }

        [Kernel]
        private static void test_short3_div([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] / b[i];
        }

        [Test]
        public void TestDiv()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_div,
                a,
                b,
                r
            );
            Assert.AreEqual(   1, r[0].s0);
            Assert.AreEqual(   1, r[0].s1);
            Assert.AreEqual(   1, r[0].s2);
            Assert.AreEqual(   0, r[1].s0);
            Assert.AreEqual(   0, r[1].s1);
            Assert.AreEqual(   0, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_div");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_div");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(   1, r[0].s0);
            Assert.AreEqual(   1, r[0].s1);
            Assert.AreEqual(   1, r[0].s2);
            Assert.AreEqual(   0, r[1].s0);
            Assert.AreEqual(   0, r[1].s1);
            Assert.AreEqual(   0, r[1].s2);
        }

        [Kernel]
        private static void test_short3_eq([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] == b[i];
        }

        [Test]
        public void TestEq()
        {
            short3[] a = new short3[] { new short3((short)   4, (short)   3, (short)   2), new short3((short)   0, (short)   1, (short)   2) };
            short3[] b = new short3[] { new short3((short)   0, (short)   1, (short)   2), new short3((short)   4, (short)   3, (short)   2) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_eq,
                a,
                b,
                r
            );
            Assert.AreEqual( 0, r[0].s0);
            Assert.AreEqual( 0, r[0].s1);
            Assert.AreEqual(-1, r[0].s2);
            Assert.AreEqual( 0, r[1].s0);
            Assert.AreEqual( 0, r[1].s1);
            Assert.AreEqual(-1, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_eq");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_eq");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual( 0, r[0].s0);
            Assert.AreEqual( 0, r[0].s1);
            Assert.AreEqual(-1, r[0].s2);
            Assert.AreEqual( 0, r[1].s0);
            Assert.AreEqual( 0, r[1].s1);
            Assert.AreEqual(-1, r[1].s2);
        }

        [Kernel]
        private static void test_short3_neq([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] != b[i];
        }

        [Test]
        public void TestNeq()
        {
            short3[] a = new short3[] { new short3((short)   4, (short)   3, (short)   2), new short3((short)   0, (short)   1, (short)   2) };
            short3[] b = new short3[] { new short3((short)   0, (short)   1, (short)   2), new short3((short)   4, (short)   3, (short)   2) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_neq,
                a,
                b,
                r
            );
            Assert.AreEqual(-1, r[0].s0);
            Assert.AreEqual(-1, r[0].s1);
            Assert.AreEqual( 0, r[0].s2);
            Assert.AreEqual(-1, r[1].s0);
            Assert.AreEqual(-1, r[1].s1);
            Assert.AreEqual( 0, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_neq");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_neq");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(-1, r[0].s0);
            Assert.AreEqual(-1, r[0].s1);
            Assert.AreEqual( 0, r[0].s2);
            Assert.AreEqual(-1, r[1].s0);
            Assert.AreEqual(-1, r[1].s1);
            Assert.AreEqual( 0, r[1].s2);
        }

        [Kernel]
        private static void test_short3_lt([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] < b[i];
        }

        [Test]
        public void TestLt()
        {
            short3[] a = new short3[] { new short3((short)   4, (short)   3, (short)   2), new short3((short)   0, (short)   1, (short)   2) };
            short3[] b = new short3[] { new short3((short)   0, (short)   1, (short)   2), new short3((short)   4, (short)   3, (short)   2) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_lt,
                a,
                b,
                r
            );
            Assert.AreEqual( 0, r[0].s0);
            Assert.AreEqual( 0, r[0].s1);
            Assert.AreEqual( 0, r[0].s2);
            Assert.AreEqual(-1, r[1].s0);
            Assert.AreEqual(-1, r[1].s1);
            Assert.AreEqual( 0, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_lt");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_lt");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual( 0, r[0].s0);
            Assert.AreEqual( 0, r[0].s1);
            Assert.AreEqual( 0, r[0].s2);
            Assert.AreEqual(-1, r[1].s0);
            Assert.AreEqual(-1, r[1].s1);
            Assert.AreEqual( 0, r[1].s2);
        }

        [Kernel]
        private static void test_short3_le([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] <= b[i];
        }

        [Test]
        public void TestLe()
        {
            short3[] a = new short3[] { new short3((short)   4, (short)   3, (short)   2), new short3((short)   0, (short)   1, (short)   2) };
            short3[] b = new short3[] { new short3((short)   0, (short)   1, (short)   2), new short3((short)   4, (short)   3, (short)   2) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_le,
                a,
                b,
                r
            );
            Assert.AreEqual( 0, r[0].s0);
            Assert.AreEqual( 0, r[0].s1);
            Assert.AreEqual(-1, r[0].s2);
            Assert.AreEqual(-1, r[1].s0);
            Assert.AreEqual(-1, r[1].s1);
            Assert.AreEqual(-1, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_le");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_le");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual( 0, r[0].s0);
            Assert.AreEqual( 0, r[0].s1);
            Assert.AreEqual(-1, r[0].s2);
            Assert.AreEqual(-1, r[1].s0);
            Assert.AreEqual(-1, r[1].s1);
            Assert.AreEqual(-1, r[1].s2);
        }

        [Kernel]
        private static void test_short3_gt([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] > b[i];
        }

        [Test]
        public void TestGt()
        {
            short3[] a = new short3[] { new short3((short)   4, (short)   3, (short)   2), new short3((short)   0, (short)   1, (short)   2) };
            short3[] b = new short3[] { new short3((short)   0, (short)   1, (short)   2), new short3((short)   4, (short)   3, (short)   2) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_gt,
                a,
                b,
                r
            );
            Assert.AreEqual(-1, r[0].s0);
            Assert.AreEqual(-1, r[0].s1);
            Assert.AreEqual( 0, r[0].s2);
            Assert.AreEqual( 0, r[1].s0);
            Assert.AreEqual( 0, r[1].s1);
            Assert.AreEqual( 0, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_gt");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_gt");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(-1, r[0].s0);
            Assert.AreEqual(-1, r[0].s1);
            Assert.AreEqual( 0, r[0].s2);
            Assert.AreEqual( 0, r[1].s0);
            Assert.AreEqual( 0, r[1].s1);
            Assert.AreEqual( 0, r[1].s2);
        }

        [Kernel]
        private static void test_short3_ge([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] >= b[i];
        }

        [Test]
        public void TestGe()
        {
            short3[] a = new short3[] { new short3((short)   4, (short)   3, (short)   2), new short3((short)   0, (short)   1, (short)   2) };
            short3[] b = new short3[] { new short3((short)   0, (short)   1, (short)   2), new short3((short)   4, (short)   3, (short)   2) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_ge,
                a,
                b,
                r
            );
            Assert.AreEqual(-1, r[0].s0);
            Assert.AreEqual(-1, r[0].s1);
            Assert.AreEqual(-1, r[0].s2);
            Assert.AreEqual( 0, r[1].s0);
            Assert.AreEqual( 0, r[1].s1);
            Assert.AreEqual(-1, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_ge");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_ge");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(-1, r[0].s0);
            Assert.AreEqual(-1, r[0].s1);
            Assert.AreEqual(-1, r[0].s2);
            Assert.AreEqual( 0, r[1].s0);
            Assert.AreEqual( 0, r[1].s1);
            Assert.AreEqual(-1, r[1].s2);
        }

        [Kernel]
        private static void test_short3_and([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] & b[i];
        }

        [Test]
        public void TestAnd()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_and,
                a,
                b,
                r
            );
            Assert.AreEqual(   5, r[0].s0);
            Assert.AreEqual(  10, r[0].s1);
            Assert.AreEqual(   5, r[0].s2);
            Assert.AreEqual(   5, r[1].s0);
            Assert.AreEqual(  10, r[1].s1);
            Assert.AreEqual(   5, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_and");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_and");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(   5, r[0].s0);
            Assert.AreEqual(  10, r[0].s1);
            Assert.AreEqual(   5, r[0].s2);
            Assert.AreEqual(   5, r[1].s0);
            Assert.AreEqual(  10, r[1].s1);
            Assert.AreEqual(   5, r[1].s2);
        }

        [Kernel]
        private static void test_short3_or([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] | b[i];
        }

        [Test]
        public void TestOr()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_or,
                a,
                b,
                r
            );
            Assert.AreEqual(   7, r[0].s0);
            Assert.AreEqual(  14, r[0].s1);
            Assert.AreEqual(  31, r[0].s2);
            Assert.AreEqual(   7, r[1].s0);
            Assert.AreEqual(  14, r[1].s1);
            Assert.AreEqual(  31, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_or");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_or");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(   7, r[0].s0);
            Assert.AreEqual(  14, r[0].s1);
            Assert.AreEqual(  31, r[0].s2);
            Assert.AreEqual(   7, r[1].s0);
            Assert.AreEqual(  14, r[1].s1);
            Assert.AreEqual(  31, r[1].s2);
        }

        [Kernel]
        private static void test_short3_xor([Global] short3[] a, [Global] short3[] b, [Global] short3[] r)
        {
            int i = Cl.GetGlobalId(0);
            r[i] = a[i] ^ b[i];
        }

        [Test]
        public void TestXor()
        {
            short3[] a = new short3[] { new short3((short)   7, (short)  14, (short)  21), new short3((short)   5, (short)  10, (short)  15) };
            short3[] b = new short3[] { new short3((short)   5, (short)  10, (short)  15), new short3((short)   7, (short)  14, (short)  21) };
            short3[] r = new short3[2];

            // test managed
            Array.Clear(r, 0, 2);
            Cl.RunKernel(
                new int[] { 2 },
                new int[] { 1 },
                (Action<short3[],short3[],short3[]>)test_short3_xor,
                a,
                b,
                r
            );
            Assert.AreEqual(   2, r[0].s0);
            Assert.AreEqual(   4, r[0].s1);
            Assert.AreEqual(  26, r[0].s2);
            Assert.AreEqual(   2, r[1].s0);
            Assert.AreEqual(   4, r[1].s1);
            Assert.AreEqual(  26, r[1].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_short3_xor");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var ma = null as Mem<short3>;
                var mb = null as Mem<short3>;
                var mr = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_short3_xor");
                    ma = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, a);
                    mb = Mem<short3>.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, b);
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, 2*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)ma);
                    kernel.SetKernelArg(1, (HandleObject)mb);
                    kernel.SetKernelArg(2, (HandleObject)mr);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 2 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, 2);
                    queue.EnqueueReadBuffer(mr, true, r);
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mb != null) mb.Dispose();
                    if (ma != null) ma.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual(   2, r[0].s0);
            Assert.AreEqual(   4, r[0].s1);
            Assert.AreEqual(  26, r[0].s2);
            Assert.AreEqual(   2, r[1].s0);
            Assert.AreEqual(   4, r[1].s1);
            Assert.AreEqual(  26, r[1].s2);
        }

        [Kernel]
        private static void test_components1([Global] short[] r, [Global] short3[] w)
        {
            short3 ar = new short3((short)1, (short)2, (short)3);
            short aw = (short)1;
            r[0] = ar.x;
            w[0].x = aw;
            r[1] = ar.y;
            w[1].y = aw;
            r[2] = ar.z;
            w[2].z = aw;
        }

        [Test]
        public void TestComponentAccessors1()
        {
            int nr = 3;
            int nw = 3;
            short[] r = new short[nr];
            short3[] w = new short3[nw];

            // test managed
            Array.Clear(r, 0, nr);
            Array.Clear(w, 0, nw);
            Cl.RunKernel(
                new int[] { 1 },
                new int[] { 1 },
                (Action<short[],short3[]>)test_components1,
                r, w
            );
            Assert.AreEqual((short)1, r[0]);
            Assert.AreEqual((short)1, w[0].s0);
            Assert.AreEqual((short)0, w[0].s1);
            Assert.AreEqual((short)0, w[0].s2);
            Assert.AreEqual((short)2, r[1]);
            Assert.AreEqual((short)1, w[1].s1);
            Assert.AreEqual((short)0, w[1].s0);
            Assert.AreEqual((short)0, w[1].s2);
            Assert.AreEqual((short)3, r[2]);
            Assert.AreEqual((short)1, w[2].s2);
            Assert.AreEqual((short)0, w[2].s0);
            Assert.AreEqual((short)0, w[2].s1);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_components1");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var mr = null as Mem<short>;
                var mw = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_components1");
                    mr = Mem<short>.CreateBuffer(context, MemFlags.WriteOnly, nr*Marshal.SizeOf<short>());
                    mw = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, nw*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)mr);
                    kernel.SetKernelArg(1, (HandleObject)mw);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 1 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, nr);
                    queue.EnqueueReadBuffer(mr, false, r);
                    Array.Clear(w, 0, nw);
                    queue.EnqueueReadBuffer(mw, false, w);
                    queue.Finish();
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mw != null) mw.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual((short)1, r[0]);
            Assert.AreEqual((short)1, w[0].s0);
            Assert.AreEqual((short)0, w[0].s1);
            Assert.AreEqual((short)0, w[0].s2);
            Assert.AreEqual((short)2, r[1]);
            Assert.AreEqual((short)1, w[1].s1);
            Assert.AreEqual((short)0, w[1].s0);
            Assert.AreEqual((short)0, w[1].s2);
            Assert.AreEqual((short)3, r[2]);
            Assert.AreEqual((short)1, w[2].s2);
            Assert.AreEqual((short)0, w[2].s0);
            Assert.AreEqual((short)0, w[2].s1);
        }
        [Kernel]
        private static void test_components2([Global] short2[] r, [Global] short3[] w)
        {
            short3 ar = new short3((short)1, (short)2, (short)3);
            short2 aw = new short2((short)1, (short)2);
            r[0] = ar.xx;
            r[1] = ar.xy;
            w[0].xy = aw;
            r[2] = ar.xz;
            w[1].xz = aw;
            r[3] = ar.yx;
            w[2].yx = aw;
            r[4] = ar.yy;
            r[5] = ar.yz;
            w[3].yz = aw;
            r[6] = ar.zx;
            w[4].zx = aw;
            r[7] = ar.zy;
            w[5].zy = aw;
            r[8] = ar.zz;
        }

        [Test]
        public void TestComponentAccessors2()
        {
            int nr = 9;
            int nw = 6;
            short2[] r = new short2[nr];
            short3[] w = new short3[nw];

            // test managed
            Array.Clear(r, 0, nr);
            Array.Clear(w, 0, nw);
            Cl.RunKernel(
                new int[] { 1 },
                new int[] { 1 },
                (Action<short2[],short3[]>)test_components2,
                r, w
            );
            Assert.AreEqual((short)1, r[0].s0);
            Assert.AreEqual((short)1, r[0].s1);
            Assert.AreEqual((short)1, r[1].s0);
            Assert.AreEqual((short)2, r[1].s1);
            Assert.AreEqual((short)1, w[0].s0);
            Assert.AreEqual((short)2, w[0].s1);
            Assert.AreEqual((short)0, w[0].s2);
            Assert.AreEqual((short)1, r[2].s0);
            Assert.AreEqual((short)3, r[2].s1);
            Assert.AreEqual((short)1, w[1].s0);
            Assert.AreEqual((short)2, w[1].s2);
            Assert.AreEqual((short)0, w[1].s1);
            Assert.AreEqual((short)2, r[3].s0);
            Assert.AreEqual((short)1, r[3].s1);
            Assert.AreEqual((short)1, w[2].s1);
            Assert.AreEqual((short)2, w[2].s0);
            Assert.AreEqual((short)0, w[2].s2);
            Assert.AreEqual((short)2, r[4].s0);
            Assert.AreEqual((short)2, r[4].s1);
            Assert.AreEqual((short)2, r[5].s0);
            Assert.AreEqual((short)3, r[5].s1);
            Assert.AreEqual((short)1, w[3].s1);
            Assert.AreEqual((short)2, w[3].s2);
            Assert.AreEqual((short)0, w[3].s0);
            Assert.AreEqual((short)3, r[6].s0);
            Assert.AreEqual((short)1, r[6].s1);
            Assert.AreEqual((short)1, w[4].s2);
            Assert.AreEqual((short)2, w[4].s0);
            Assert.AreEqual((short)0, w[4].s1);
            Assert.AreEqual((short)3, r[7].s0);
            Assert.AreEqual((short)2, r[7].s1);
            Assert.AreEqual((short)1, w[5].s2);
            Assert.AreEqual((short)2, w[5].s1);
            Assert.AreEqual((short)0, w[5].s0);
            Assert.AreEqual((short)3, r[8].s0);
            Assert.AreEqual((short)3, r[8].s1);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_components2");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var mr = null as Mem<short2>;
                var mw = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_components2");
                    mr = Mem<short2>.CreateBuffer(context, MemFlags.WriteOnly, nr*Marshal.SizeOf<short2>());
                    mw = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, nw*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)mr);
                    kernel.SetKernelArg(1, (HandleObject)mw);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 1 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, nr);
                    queue.EnqueueReadBuffer(mr, false, r);
                    Array.Clear(w, 0, nw);
                    queue.EnqueueReadBuffer(mw, false, w);
                    queue.Finish();
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mw != null) mw.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual((short)1, r[0].s0);
            Assert.AreEqual((short)1, r[0].s1);
            Assert.AreEqual((short)1, r[1].s0);
            Assert.AreEqual((short)2, r[1].s1);
            Assert.AreEqual((short)1, w[0].s0);
            Assert.AreEqual((short)2, w[0].s1);
            Assert.AreEqual((short)0, w[0].s2);
            Assert.AreEqual((short)1, r[2].s0);
            Assert.AreEqual((short)3, r[2].s1);
            Assert.AreEqual((short)1, w[1].s0);
            Assert.AreEqual((short)2, w[1].s2);
            Assert.AreEqual((short)0, w[1].s1);
            Assert.AreEqual((short)2, r[3].s0);
            Assert.AreEqual((short)1, r[3].s1);
            Assert.AreEqual((short)1, w[2].s1);
            Assert.AreEqual((short)2, w[2].s0);
            Assert.AreEqual((short)0, w[2].s2);
            Assert.AreEqual((short)2, r[4].s0);
            Assert.AreEqual((short)2, r[4].s1);
            Assert.AreEqual((short)2, r[5].s0);
            Assert.AreEqual((short)3, r[5].s1);
            Assert.AreEqual((short)1, w[3].s1);
            Assert.AreEqual((short)2, w[3].s2);
            Assert.AreEqual((short)0, w[3].s0);
            Assert.AreEqual((short)3, r[6].s0);
            Assert.AreEqual((short)1, r[6].s1);
            Assert.AreEqual((short)1, w[4].s2);
            Assert.AreEqual((short)2, w[4].s0);
            Assert.AreEqual((short)0, w[4].s1);
            Assert.AreEqual((short)3, r[7].s0);
            Assert.AreEqual((short)2, r[7].s1);
            Assert.AreEqual((short)1, w[5].s2);
            Assert.AreEqual((short)2, w[5].s1);
            Assert.AreEqual((short)0, w[5].s0);
            Assert.AreEqual((short)3, r[8].s0);
            Assert.AreEqual((short)3, r[8].s1);
        }
        [Kernel]
        private static void test_components3([Global] short3[] r, [Global] short3[] w)
        {
            short3 ar = new short3((short)1, (short)2, (short)3);
            short3 aw = new short3((short)1, (short)2, (short)3);
            r[0] = ar.xxx;
            r[1] = ar.xxy;
            r[2] = ar.xxz;
            r[3] = ar.xyx;
            r[4] = ar.xyy;
            r[5] = ar.xyz;
            w[0].xyz = aw;
            r[6] = ar.xzx;
            r[7] = ar.xzy;
            w[1].xzy = aw;
            r[8] = ar.xzz;
            r[9] = ar.yxx;
            r[10] = ar.yxy;
            r[11] = ar.yxz;
            w[2].yxz = aw;
            r[12] = ar.yyx;
            r[13] = ar.yyy;
            r[14] = ar.yyz;
            r[15] = ar.yzx;
            w[3].yzx = aw;
            r[16] = ar.yzy;
            r[17] = ar.yzz;
            r[18] = ar.zxx;
            r[19] = ar.zxy;
            w[4].zxy = aw;
            r[20] = ar.zxz;
            r[21] = ar.zyx;
            w[5].zyx = aw;
            r[22] = ar.zyy;
            r[23] = ar.zyz;
            r[24] = ar.zzx;
            r[25] = ar.zzy;
            r[26] = ar.zzz;
        }

        [Test]
        public void TestComponentAccessors3()
        {
            int nr = 27;
            int nw = 6;
            short3[] r = new short3[nr];
            short3[] w = new short3[nw];

            // test managed
            Array.Clear(r, 0, nr);
            Array.Clear(w, 0, nw);
            Cl.RunKernel(
                new int[] { 1 },
                new int[] { 1 },
                (Action<short3[],short3[]>)test_components3,
                r, w
            );
            Assert.AreEqual((short)1, r[0].s0);
            Assert.AreEqual((short)1, r[0].s1);
            Assert.AreEqual((short)1, r[0].s2);
            Assert.AreEqual((short)1, r[1].s0);
            Assert.AreEqual((short)1, r[1].s1);
            Assert.AreEqual((short)2, r[1].s2);
            Assert.AreEqual((short)1, r[2].s0);
            Assert.AreEqual((short)1, r[2].s1);
            Assert.AreEqual((short)3, r[2].s2);
            Assert.AreEqual((short)1, r[3].s0);
            Assert.AreEqual((short)2, r[3].s1);
            Assert.AreEqual((short)1, r[3].s2);
            Assert.AreEqual((short)1, r[4].s0);
            Assert.AreEqual((short)2, r[4].s1);
            Assert.AreEqual((short)2, r[4].s2);
            Assert.AreEqual((short)1, r[5].s0);
            Assert.AreEqual((short)2, r[5].s1);
            Assert.AreEqual((short)3, r[5].s2);
            Assert.AreEqual((short)1, w[0].s0);
            Assert.AreEqual((short)2, w[0].s1);
            Assert.AreEqual((short)3, w[0].s2);
            Assert.AreEqual((short)1, r[6].s0);
            Assert.AreEqual((short)3, r[6].s1);
            Assert.AreEqual((short)1, r[6].s2);
            Assert.AreEqual((short)1, r[7].s0);
            Assert.AreEqual((short)3, r[7].s1);
            Assert.AreEqual((short)2, r[7].s2);
            Assert.AreEqual((short)1, w[1].s0);
            Assert.AreEqual((short)2, w[1].s2);
            Assert.AreEqual((short)3, w[1].s1);
            Assert.AreEqual((short)1, r[8].s0);
            Assert.AreEqual((short)3, r[8].s1);
            Assert.AreEqual((short)3, r[8].s2);
            Assert.AreEqual((short)2, r[9].s0);
            Assert.AreEqual((short)1, r[9].s1);
            Assert.AreEqual((short)1, r[9].s2);
            Assert.AreEqual((short)2, r[10].s0);
            Assert.AreEqual((short)1, r[10].s1);
            Assert.AreEqual((short)2, r[10].s2);
            Assert.AreEqual((short)2, r[11].s0);
            Assert.AreEqual((short)1, r[11].s1);
            Assert.AreEqual((short)3, r[11].s2);
            Assert.AreEqual((short)1, w[2].s1);
            Assert.AreEqual((short)2, w[2].s0);
            Assert.AreEqual((short)3, w[2].s2);
            Assert.AreEqual((short)2, r[12].s0);
            Assert.AreEqual((short)2, r[12].s1);
            Assert.AreEqual((short)1, r[12].s2);
            Assert.AreEqual((short)2, r[13].s0);
            Assert.AreEqual((short)2, r[13].s1);
            Assert.AreEqual((short)2, r[13].s2);
            Assert.AreEqual((short)2, r[14].s0);
            Assert.AreEqual((short)2, r[14].s1);
            Assert.AreEqual((short)3, r[14].s2);
            Assert.AreEqual((short)2, r[15].s0);
            Assert.AreEqual((short)3, r[15].s1);
            Assert.AreEqual((short)1, r[15].s2);
            Assert.AreEqual((short)1, w[3].s1);
            Assert.AreEqual((short)2, w[3].s2);
            Assert.AreEqual((short)3, w[3].s0);
            Assert.AreEqual((short)2, r[16].s0);
            Assert.AreEqual((short)3, r[16].s1);
            Assert.AreEqual((short)2, r[16].s2);
            Assert.AreEqual((short)2, r[17].s0);
            Assert.AreEqual((short)3, r[17].s1);
            Assert.AreEqual((short)3, r[17].s2);
            Assert.AreEqual((short)3, r[18].s0);
            Assert.AreEqual((short)1, r[18].s1);
            Assert.AreEqual((short)1, r[18].s2);
            Assert.AreEqual((short)3, r[19].s0);
            Assert.AreEqual((short)1, r[19].s1);
            Assert.AreEqual((short)2, r[19].s2);
            Assert.AreEqual((short)1, w[4].s2);
            Assert.AreEqual((short)2, w[4].s0);
            Assert.AreEqual((short)3, w[4].s1);
            Assert.AreEqual((short)3, r[20].s0);
            Assert.AreEqual((short)1, r[20].s1);
            Assert.AreEqual((short)3, r[20].s2);
            Assert.AreEqual((short)3, r[21].s0);
            Assert.AreEqual((short)2, r[21].s1);
            Assert.AreEqual((short)1, r[21].s2);
            Assert.AreEqual((short)1, w[5].s2);
            Assert.AreEqual((short)2, w[5].s1);
            Assert.AreEqual((short)3, w[5].s0);
            Assert.AreEqual((short)3, r[22].s0);
            Assert.AreEqual((short)2, r[22].s1);
            Assert.AreEqual((short)2, r[22].s2);
            Assert.AreEqual((short)3, r[23].s0);
            Assert.AreEqual((short)2, r[23].s1);
            Assert.AreEqual((short)3, r[23].s2);
            Assert.AreEqual((short)3, r[24].s0);
            Assert.AreEqual((short)3, r[24].s1);
            Assert.AreEqual((short)1, r[24].s2);
            Assert.AreEqual((short)3, r[25].s0);
            Assert.AreEqual((short)3, r[25].s1);
            Assert.AreEqual((short)2, r[25].s2);
            Assert.AreEqual((short)3, r[26].s0);
            Assert.AreEqual((short)3, r[26].s1);
            Assert.AreEqual((short)3, r[26].s2);

            // compile kernel
            var source = ClCompiler.EmitKernel("opencl-tests.dll", "OpenCl.Tests.TestShort3", "test_components3");

            // test native
            Platform platform = Platform.GetPlatformIDs()[0];
            Device[] devices = Device.GetDeviceIDs(platform, DeviceType.Cpu);
            using (var context = Context.CreateContext(platform, devices, null, null))
            using (var queue = CommandQueue.CreateCommandQueue(context, devices[0]))
            {
                var program = null as Program;
                var kernel = null as Kernel;
                var mr = null as Mem<short3>;
                var mw = null as Mem<short3>;
                try {
                    program = Program.CreateProgramWithSource(context, new String[] { source });
                    try { program.BuildProgram(devices, null, null, null); } catch (OpenClException ex) { Console.WriteLine(source); throw ex; }
                    kernel = Kernel.CreateKernel(program, "test_components3");
                    mr = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, nr*Marshal.SizeOf<short3>());
                    mw = Mem<short3>.CreateBuffer(context, MemFlags.WriteOnly, nw*Marshal.SizeOf<short3>());
                    kernel.SetKernelArg(0, (HandleObject)mr);
                    kernel.SetKernelArg(1, (HandleObject)mw);
                    queue.EnqueueNDRangeKernel(kernel, null, new int[] { 1 }, null, null);
                    queue.Finish();
                    Array.Clear(r, 0, nr);
                    queue.EnqueueReadBuffer(mr, false, r);
                    Array.Clear(w, 0, nw);
                    queue.EnqueueReadBuffer(mw, false, w);
                    queue.Finish();
                }
                finally {
                    if (mr != null) mr.Dispose();
                    if (mw != null) mw.Dispose();
                    if (kernel != null) kernel.Dispose();
                    if (program != null) program.Dispose();
                }
            }
            Assert.AreEqual((short)1, r[0].s0);
            Assert.AreEqual((short)1, r[0].s1);
            Assert.AreEqual((short)1, r[0].s2);
            Assert.AreEqual((short)1, r[1].s0);
            Assert.AreEqual((short)1, r[1].s1);
            Assert.AreEqual((short)2, r[1].s2);
            Assert.AreEqual((short)1, r[2].s0);
            Assert.AreEqual((short)1, r[2].s1);
            Assert.AreEqual((short)3, r[2].s2);
            Assert.AreEqual((short)1, r[3].s0);
            Assert.AreEqual((short)2, r[3].s1);
            Assert.AreEqual((short)1, r[3].s2);
            Assert.AreEqual((short)1, r[4].s0);
            Assert.AreEqual((short)2, r[4].s1);
            Assert.AreEqual((short)2, r[4].s2);
            Assert.AreEqual((short)1, r[5].s0);
            Assert.AreEqual((short)2, r[5].s1);
            Assert.AreEqual((short)3, r[5].s2);
            Assert.AreEqual((short)1, w[0].s0);
            Assert.AreEqual((short)2, w[0].s1);
            Assert.AreEqual((short)3, w[0].s2);
            Assert.AreEqual((short)1, r[6].s0);
            Assert.AreEqual((short)3, r[6].s1);
            Assert.AreEqual((short)1, r[6].s2);
            Assert.AreEqual((short)1, r[7].s0);
            Assert.AreEqual((short)3, r[7].s1);
            Assert.AreEqual((short)2, r[7].s2);
            Assert.AreEqual((short)1, w[1].s0);
            Assert.AreEqual((short)2, w[1].s2);
            Assert.AreEqual((short)3, w[1].s1);
            Assert.AreEqual((short)1, r[8].s0);
            Assert.AreEqual((short)3, r[8].s1);
            Assert.AreEqual((short)3, r[8].s2);
            Assert.AreEqual((short)2, r[9].s0);
            Assert.AreEqual((short)1, r[9].s1);
            Assert.AreEqual((short)1, r[9].s2);
            Assert.AreEqual((short)2, r[10].s0);
            Assert.AreEqual((short)1, r[10].s1);
            Assert.AreEqual((short)2, r[10].s2);
            Assert.AreEqual((short)2, r[11].s0);
            Assert.AreEqual((short)1, r[11].s1);
            Assert.AreEqual((short)3, r[11].s2);
            Assert.AreEqual((short)1, w[2].s1);
            Assert.AreEqual((short)2, w[2].s0);
            Assert.AreEqual((short)3, w[2].s2);
            Assert.AreEqual((short)2, r[12].s0);
            Assert.AreEqual((short)2, r[12].s1);
            Assert.AreEqual((short)1, r[12].s2);
            Assert.AreEqual((short)2, r[13].s0);
            Assert.AreEqual((short)2, r[13].s1);
            Assert.AreEqual((short)2, r[13].s2);
            Assert.AreEqual((short)2, r[14].s0);
            Assert.AreEqual((short)2, r[14].s1);
            Assert.AreEqual((short)3, r[14].s2);
            Assert.AreEqual((short)2, r[15].s0);
            Assert.AreEqual((short)3, r[15].s1);
            Assert.AreEqual((short)1, r[15].s2);
            Assert.AreEqual((short)1, w[3].s1);
            Assert.AreEqual((short)2, w[3].s2);
            Assert.AreEqual((short)3, w[3].s0);
            Assert.AreEqual((short)2, r[16].s0);
            Assert.AreEqual((short)3, r[16].s1);
            Assert.AreEqual((short)2, r[16].s2);
            Assert.AreEqual((short)2, r[17].s0);
            Assert.AreEqual((short)3, r[17].s1);
            Assert.AreEqual((short)3, r[17].s2);
            Assert.AreEqual((short)3, r[18].s0);
            Assert.AreEqual((short)1, r[18].s1);
            Assert.AreEqual((short)1, r[18].s2);
            Assert.AreEqual((short)3, r[19].s0);
            Assert.AreEqual((short)1, r[19].s1);
            Assert.AreEqual((short)2, r[19].s2);
            Assert.AreEqual((short)1, w[4].s2);
            Assert.AreEqual((short)2, w[4].s0);
            Assert.AreEqual((short)3, w[4].s1);
            Assert.AreEqual((short)3, r[20].s0);
            Assert.AreEqual((short)1, r[20].s1);
            Assert.AreEqual((short)3, r[20].s2);
            Assert.AreEqual((short)3, r[21].s0);
            Assert.AreEqual((short)2, r[21].s1);
            Assert.AreEqual((short)1, r[21].s2);
            Assert.AreEqual((short)1, w[5].s2);
            Assert.AreEqual((short)2, w[5].s1);
            Assert.AreEqual((short)3, w[5].s0);
            Assert.AreEqual((short)3, r[22].s0);
            Assert.AreEqual((short)2, r[22].s1);
            Assert.AreEqual((short)2, r[22].s2);
            Assert.AreEqual((short)3, r[23].s0);
            Assert.AreEqual((short)2, r[23].s1);
            Assert.AreEqual((short)3, r[23].s2);
            Assert.AreEqual((short)3, r[24].s0);
            Assert.AreEqual((short)3, r[24].s1);
            Assert.AreEqual((short)1, r[24].s2);
            Assert.AreEqual((short)3, r[25].s0);
            Assert.AreEqual((short)3, r[25].s1);
            Assert.AreEqual((short)2, r[25].s2);
            Assert.AreEqual((short)3, r[26].s0);
            Assert.AreEqual((short)3, r[26].s1);
            Assert.AreEqual((short)3, r[26].s2);
        }
    }
}

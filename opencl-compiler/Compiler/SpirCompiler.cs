﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace OpenCl.Compiler
{
    public partial class SpirCompiler
    {
        // compiles into SPIR-V Format
        // see: https://www.khronos.org/registry/spir-v/specs/1.1/SPIRV.html

        private static readonly Dictionary<string,string> typeMap = new Dictionary<string,string>()
        {
            { "System.Void",     "void" },

            { "System.SByte",    "char" },
            { "System.Int8",     "char" },
            { "System.Byte",     "uchar" },
            { "System.UInt8",    "uchar" },
            { "System.Int16",    "short" },
            { "System.UInt16",   "ushort" },
            { "System.Int32",    "int" },
            { "System.UInt32",   "uint" },
            { "System.Int64",    "long" },
            { "System.UInt64",   "ulong" },
            { "System.Single",   "float" },
            { "System.Double",   "double" },

            { "OpenCl.sbyte2",   "char2" },
            { "OpenCl.sbyte3",   "char3" },
            { "OpenCl.sbyte4",   "char4" },
            { "OpenCl.sbyte8",   "char8" },
            { "OpenCl.sbyte16",  "char16" },
            { "OpenCl.byte2",    "uchar2" },
            { "OpenCl.byte3",    "uchar3" },
            { "OpenCl.byte4",    "uchar4" },
            { "OpenCl.byte8",    "uchar8" },
            { "OpenCl.byte16",   "uchar16" },
            { "OpenCl.short2",   "short2" },
            { "OpenCl.short3",   "short3" },
            { "OpenCl.short4",   "short4" },
            { "OpenCl.short8",   "short8" },
            { "OpenCl.short16",  "short16" },
            { "OpenCl.ushort2",  "ushort2" },
            { "OpenCl.ushort3",  "ushort3" },
            { "OpenCl.ushort4",  "ushort4" },
            { "OpenCl.ushort8",  "ushort8" },
            { "OpenCl.ushort16", "ushort16" },
            { "OpenCl.int2",     "int2" },
            { "OpenCl.int3",     "int3" },
            { "OpenCl.int4",     "int4" },
            { "OpenCl.int8",     "int8" },
            { "OpenCl.int16",    "int16" },
            { "OpenCl.uint2",    "uint2" },
            { "OpenCl.uint3",    "uint3" },
            { "OpenCl.uint4",    "uint4" },
            { "OpenCl.uint8",    "uint8" },
            { "OpenCl.uint16",   "uint16" },
            { "OpenCl.long2",    "long2" },
            { "OpenCl.long3",    "long3" },
            { "OpenCl.long4",    "long4" },
            { "OpenCl.long8",    "long8" },
            { "OpenCl.long16",   "long16" },
            { "OpenCl.ulong2",   "ulong2" },
            { "OpenCl.ulong3",   "ulong3" },
            { "OpenCl.ulong4",   "ulong4" },
            { "OpenCl.ulong8",   "ulong8" },
            { "OpenCl.ulong16",  "ulong16" },
            { "OpenCl.float2",   "float2" },
            { "OpenCl.float3",   "float3" },
            { "OpenCl.float4",   "float4" },
            { "OpenCl.float8",   "float8" },
            { "OpenCl.float16",  "float16" },
            { "OpenCl.double2",  "double2" },
            { "OpenCl.double3",  "double3" },
            { "OpenCl.double4",  "double4" },
            { "OpenCl.double8",  "double8" },
            { "OpenCl.double16", "double16" },

            { "System.SByte[]",    "char*" },
            { "System.Int8[]",     "char*" },
            { "System.Byte[]",     "uchar*" },
            { "System.UInt8[]",    "uchar*" },
            { "System.Int16[]",    "short*" },
            { "System.UInt16[]",   "ushort*" },
            { "System.Int32[]",    "int*" },
            { "System.UInt32[]",   "uint*" },
            { "System.Int64[]",    "long*" },
            { "System.UInt64[]",   "ulong*" },
            { "System.Single[]",   "float*" },
            { "System.Double[]",   "double*" },

            { "OpenCl.sbyte2[]",   "char2*" },
            { "OpenCl.sbyte3[]",   "char3*" },
            { "OpenCl.sbyte4[]",   "char4*" },
            { "OpenCl.sbyte8[]",   "char8*" },
            { "OpenCl.sbyte16[]",  "char16*" },
            { "OpenCl.byte2[]",    "uchar2*" },
            { "OpenCl.byte3[]",    "uchar3*" },
            { "OpenCl.byte4[]",    "uchar4*" },
            { "OpenCl.byte8[]",    "uchar8*" },
            { "OpenCl.byte16[]",   "uchar16*" },
            { "OpenCl.short2[]",   "short2*" },
            { "OpenCl.short3[]",   "short3*" },
            { "OpenCl.short4[]",   "short4*" },
            { "OpenCl.short8[]",   "short8*" },
            { "OpenCl.short16[]",  "short16*" },
            { "OpenCl.ushort2[]",  "ushort2*" },
            { "OpenCl.ushort3[]",  "ushort3*" },
            { "OpenCl.ushort4[]",  "ushort4*" },
            { "OpenCl.ushort8[]",  "ushort8*" },
            { "OpenCl.ushort16[]", "ushort16*" },
            { "OpenCl.int2[]",     "int2*" },
            { "OpenCl.int3[]",     "int3*" },
            { "OpenCl.int4[]",     "int4*" },
            { "OpenCl.int8[]",     "int8*" },
            { "OpenCl.int16[]",    "int16*" },
            { "OpenCl.uint2[]",    "uint2*" },
            { "OpenCl.uint3[]",    "uint3*" },
            { "OpenCl.uint4[]",    "uint4*" },
            { "OpenCl.uint8[]",    "uint8*" },
            { "OpenCl.uint16[]",   "uint16*" },
            { "OpenCl.long2[]",    "long2*" },
            { "OpenCl.long3[]",    "long3*" },
            { "OpenCl.long4[]",    "long4*" },
            { "OpenCl.long8[]",    "long8*" },
            { "OpenCl.long16[]",   "long16*" },
            { "OpenCl.ulong2[]",   "ulong2*" },
            { "OpenCl.ulong3[]",   "ulong3*" },
            { "OpenCl.ulong4[]",   "ulong4*" },
            { "OpenCl.ulong8[]",   "ulong8*" },
            { "OpenCl.ulong16[]",  "ulong16*" },
            { "OpenCl.float2[]",   "float2*" },
            { "OpenCl.float3[]",   "float3*" },
            { "OpenCl.float4[]",   "float4*" },
            { "OpenCl.float8[]",   "float8*" },
            { "OpenCl.float16[]",  "float16*" },
            { "OpenCl.double2[]",  "double2*" },
            { "OpenCl.double3[]",  "double3*" },
            { "OpenCl.double4[]",  "double4*" },
            { "OpenCl.double8[]",  "double8*" },
            { "OpenCl.double16[]", "double16*" },

            { "System.SByte*",    "char*" },
            { "System.Int8*",     "char*" },
            { "System.Byte*",     "uchar*" },
            { "System.UInt8*",    "uchar*" },
            { "System.Int16*",    "short*" },
            { "System.UInt16*",   "ushort*" },
            { "System.Int32*",    "int*" },
            { "System.UInt32*",   "uint*" },
            { "System.Int64*",    "long*" },
            { "System.UInt64*",   "ulong*" },
            { "System.Single*",   "float*" },
            { "System.Double*",   "double*" },

            { "OpenCl.sbyte2*",   "char2*" },
            { "OpenCl.sbyte3*",   "char3*" },
            { "OpenCl.sbyte4*",   "char4*" },
            { "OpenCl.sbyte8*",   "char8*" },
            { "OpenCl.sbyte16*",  "char16*" },
            { "OpenCl.byte2*",    "uchar2*" },
            { "OpenCl.byte3*",    "uchar3*" },
            { "OpenCl.byte4*",    "uchar4*" },
            { "OpenCl.byte8*",    "uchar8*" },
            { "OpenCl.byte16*",   "uchar16*" },
            { "OpenCl.short2*",   "short2*" },
            { "OpenCl.short3*",   "short3*" },
            { "OpenCl.short4*",   "short4*" },
            { "OpenCl.short8*",   "short8*" },
            { "OpenCl.short16*",  "short16*" },
            { "OpenCl.ushort2*",  "ushort2*" },
            { "OpenCl.ushort3*",  "ushort3*" },
            { "OpenCl.ushort4*",  "ushort4*" },
            { "OpenCl.ushort8*",  "ushort8*" },
            { "OpenCl.ushort16*", "ushort16*" },
            { "OpenCl.int2*",     "int2*" },
            { "OpenCl.int3*",     "int3*" },
            { "OpenCl.int4*",     "int4*" },
            { "OpenCl.int8*",     "int8*" },
            { "OpenCl.int16*",    "int16*" },
            { "OpenCl.uint2*",    "uint2*" },
            { "OpenCl.uint3*",    "uint3*" },
            { "OpenCl.uint4*",    "uint4*" },
            { "OpenCl.uint8*",    "uint8*" },
            { "OpenCl.uint16*",   "uint16*" },
            { "OpenCl.long2*",    "long2*" },
            { "OpenCl.long3*",    "long3*" },
            { "OpenCl.long4*",    "long4*" },
            { "OpenCl.long8*",    "long8*" },
            { "OpenCl.long16*",   "long16*" },
            { "OpenCl.ulong2*",   "ulong2*" },
            { "OpenCl.ulong3*",   "ulong3*" },
            { "OpenCl.ulong4*",   "ulong4*" },
            { "OpenCl.ulong8*",   "ulong8*" },
            { "OpenCl.ulong16*",  "ulong16*" },
            { "OpenCl.float2*",   "float2*" },
            { "OpenCl.float3*",   "float3*" },
            { "OpenCl.float4*",   "float4*" },
            { "OpenCl.float8*",   "float8*" },
            { "OpenCl.float16*",  "float16*" },
            { "OpenCl.double2*",  "double2*" },
            { "OpenCl.double3*",  "double3*" },
            { "OpenCl.double4*",  "double4*" },
            { "OpenCl.double8*",  "double8*" },
            { "OpenCl.double16*", "double16*" },
        };

        // IL source data

        // private readonly ModuleDefinition module;
        // private readonly TypeDefinition type;
        private readonly Queue<MethodDefinition> queue;

        // SPIR-V target data

        private int rcount;

        private List<OpEntryPoint> entryPoints;
        private List<OpDecorate> decorations;
        private Dictionary<TypeOpCode,int> types;
        private List<TypeOpCode> types_list;
        private Dictionary<string,TypedResultOpCode> imports;
        private Dictionary<OpConstant,int> constants;
        private List<List<SpirOpCode>> functions;

        // constructor

        private SpirCompiler(/*ModuleDefinition module, TypeDefinition type,*/ params MethodDefinition[] methods)
        {
            // this.module = module;
            // this.type = type;
            this.queue = new Queue<MethodDefinition>(methods);
            this.rcount = 1;
            this.entryPoints = new List<OpEntryPoint>();
            this.decorations = new List<OpDecorate>();
            this.types = new Dictionary<TypeOpCode,int>();
            this.types_list = new List<TypeOpCode>();
            this.imports = new Dictionary<String,TypedResultOpCode>();
            this.constants = new Dictionary<OpConstant,int>();
            this.functions = new List<List<SpirOpCode>>();
        }

        // type helpers

        private int SpirTypeIdCallback(TypeOpCode type)
        {
            int id;
            if (!this.types.TryGetValue(type, out id)) {
                id = this.rcount++;
                this.types.Add(type, id);
                this.types_list.Add(type);
            }
            return id;
        }

        private TypeOpCode GetTypeOpCode(String name, StorageClass storage)
        {
            switch (name) {
                case "System.Void":
                    return new OpTypeVoid(SpirTypeIdCallback);
                case "System.Int8":
                case "System.SByte":
                    return new OpTypeInt(SpirTypeIdCallback, 8);
                case "System.Int16":
                    return new OpTypeInt(SpirTypeIdCallback, 16);
                case "System.Int32":
                    return new OpTypeInt(SpirTypeIdCallback, 32);
                case "System.Int64":
                    return new OpTypeInt(SpirTypeIdCallback, 64);
                case "System.IntPtr":
                    return new OpTypeInt(SpirTypeIdCallback, 8*Marshal.SizeOf<IntPtr>());
                case "System.Single":
                    return new OpTypeFloat(SpirTypeIdCallback, 32);
                case "System.Double":
                    return new OpTypeFloat(SpirTypeIdCallback, 64);
                case "System.Int32[]":
                    return new OpTypePointer(SpirTypeIdCallback, storage, new OpTypeInt(SpirTypeIdCallback, 32));
                case "OpenCl.int4[]":
                    return new OpTypePointer(SpirTypeIdCallback, storage,
                                new OpTypeVector(SpirTypeIdCallback, 4,
                                    new OpTypeInt(SpirTypeIdCallback, 32)));
                default:
                    throw new CompilerException(String.Format("*** Error: unsupported type '{0}'.", name));
            }
        }

        private TypeOpCode GetTypeOpCode<T>()
        {
            return GetTypeOpCode<T>((StorageClass)0);
        }

        private TypeOpCode GetTypeOpCode<T>(StorageClass storage)
        {
            return GetTypeOpCode(typeof(T).FullName, storage);
        }

        private TypeOpCode GetTypeOpCode(TypeReference td)
        {
            return GetTypeOpCode(td, StorageClass.Function);
        }

        private TypeOpCode GetTypeOpCode(TypeReference td, StorageClass storage)
        {
            return GetTypeOpCode(td.FullName, storage);
        }

        private TypeOpCode GetTypeOpCode(ParameterReference pd)
        {
            var global = pd.Resolve().CustomAttributes.Where(ai => ai.AttributeType.FullName == "OpenCl.GlobalAttribute").Count() == 1;
            return GetTypeOpCode(pd.ParameterType);
        }

        private TypeOpCode GetTypeOpCode(VariableReference vd)
        {
            return GetTypeOpCode(vd.VariableType);
        }

        private OpTypeFunction GetTypeOpCode(MethodReference md)
        {
            var r = GetTypeOpCode(md.ReturnType);
            var p = md.Parameters
                    .Select(pi => GetTypeOpCode(pi))
                    .ToArray();
            return new OpTypeFunction(SpirTypeIdCallback, r, p);
        }

        // private static readonly Dictionary<TypeOpCode,Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>> _convert_ops = new Dictionary<TypeOpCode,Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>>()
        // {
        //     { new OpTypeInt(t => -1, 8), new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>() {
        //         { typeof(sbyte),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
        //         { typeof(short),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
        //         { typeof(int),    (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
        //         { typeof(long),   (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
        //         { typeof(IntPtr), (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
        //         { typeof(float),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
        //         { typeof(double), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) }
        //     }},
        //     { new OpTypeInt(t => -1, 16), new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>() {
        //         { typeof(sbyte),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) },
        //         { typeof(short),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) },
        //         { typeof(int),    (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) },
        //         { typeof(long),   (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) },
        //         { typeof(IntPtr), (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) },
        //         { typeof(float),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) },
        //         { typeof(double), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<short>(), value) }
        //     }},
        //     { new OpTypeInt(t => -1, 32), new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>() {
        //         { typeof(sbyte),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) },
        //         { typeof(short),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) },
        //         { typeof(int),    (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) },
        //         { typeof(long),   (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) },
        //         { typeof(IntPtr), (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) },
        //         { typeof(float),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) },
        //         { typeof(double), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<int>(), value) }
        //     }},
        //     { new OpTypeInt(t => -1, 64), new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>() {
        //         { typeof(sbyte),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) },
        //         { typeof(short),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) },
        //         { typeof(int),    (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) },
        //         { typeof(long),   (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) },
        //         { typeof(IntPtr), (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) },
        //         { typeof(float),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) },
        //         { typeof(double), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<long>(), value) }
        //     }},
        //     { new OpTypeFloat(t => -1, 32), new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>() {
        //         { typeof(sbyte),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) },
        //         { typeof(short),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) },
        //         { typeof(int),    (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) },
        //         { typeof(long),   (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) },
        //         { typeof(IntPtr), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) },
        //         { typeof(float),  (compiler, value) => new OpFConvert(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) },
        //         { typeof(double), (compiler, value) => new OpFConvert(compiler.rcount++, compiler.GetTypeOpCode<float>(), value) }
        //     }},
        //     { new OpTypeFloat(t => -1, 64), new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>>() {
        //         { typeof(sbyte),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) },
        //         { typeof(short),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) },
        //         { typeof(int),    (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) },
        //         { typeof(long),   (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) },
        //         { typeof(IntPtr), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) },
        //         { typeof(float),  (compiler, value) => new OpFConvert(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) },
        //         { typeof(double), (compiler, value) => new OpFConvert(compiler.rcount++, compiler.GetTypeOpCode<double>(), value) }
        //     }}
        // };

        // private TypedResultOpCode GetConversionOpCode(TypedResultOpCode src, Type dst)
        // {
        //     Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode>> map = null;
        //     if (!_convert_ops.TryGetValue(src.ResultType, out map)) {
        //         throw new CompilerException(String.Format("Unsupported source type in type conversion: {0} -> {1}.", src.ResultType, dst));
        //     }
        //     Func<SpirCompiler,TypedResultOpCode,TypedResultOpCode> factory = null;
        //     if (!map.TryGetValue(dst, out factory)) {
        //         throw new CompilerException(String.Format("Unsupported target type in type conversion: {0} -> {1}.", src.ResultType, dst));
        //     }
        //     return factory(this, src);
        // }

        private static readonly Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,ConversionOpCode>> _convert_ops = new Dictionary<Type,Func<SpirCompiler,TypedResultOpCode,ConversionOpCode>>()
        {
            { typeof(sbyte),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
            { typeof(short),  (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
            { typeof(int),    (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
            { typeof(long),   (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
            { typeof(IntPtr), (compiler, value) => new OpSConvert(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
            { typeof(float),  (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) },
            { typeof(double), (compiler, value) => new OpConvertFToS(compiler.rcount++, compiler.GetTypeOpCode<sbyte>(), value) }
        };

        private ConversionOpCode GetConversionOpCode(TypedResultOpCode src, Type dst)
        {
            Func<SpirCompiler,TypedResultOpCode,ConversionOpCode> factory = null;
            if (!_convert_ops.TryGetValue(dst, out factory)) {
                throw new CompilerException(String.Format("Unsupported type conversion: {0} -> {1}.", src.ResultType, dst));
            }
            return factory(this, src);
        }

        // constant helper

        private int SpirConstantCallback(OpConstant op)
        {
            int id;
            if (!this.constants.TryGetValue(op, out id)) {
                id = this.rcount++;
                this.constants.Add(op, id);
            }
            return id;
        }

        private string GetMethodName(MethodDefinition mdef)
        {
            var name =
                mdef.CustomAttributes
                .Where(ai => ai.AttributeType.FullName == "OpenCl.ClNameAttribute")
                .Select((attr, idx) => attr.ConstructorArguments[0].Value as string)
                .FirstOrDefault();
            return name != null ? name : mdef.Name;
        }

        private void Parse(MethodDefinition method)
        {
            var functionType = GetTypeOpCode(method);
            OpFunction function = new OpFunction(this.rcount++, functionType);
            if (method.CustomAttributes.SingleOrDefault(ai => ai.AttributeType.FullName == "OpenCl.KernelAttribute") != null) {
                this.entryPoints.Add(new OpEntryPoint(ExecutionModel.Kernel, function, GetMethodName(method)));
            }
            List<SpirOpCode> funcdef = new List<SpirOpCode>();
            funcdef.Add(function);
            var nparams = method.Parameters.Count;
            var param = new OpFunctionParameter[nparams];
            for (var i=0; i<nparams; i++) {
                var pi = method.Parameters[i];
                var ci = new OpFunctionParameter(this.rcount++, GetTypeOpCode(pi));
                param[i] = ci;
                funcdef.Add(ci);
            }



            var body = method.Body;
            var vars = new TypedResultOpCode[body.Variables.Count];
            // foreach (var v in vars) {
            //     string name = null;
            //     if (!typeMap.TryGetValue(v.VariableType.FullName, out name)) {
            //         throw new ArgumentException(String.Format("Unsupported type: {0}.", v.VariableType.FullName));
            //     }
            //     this.builder.AppendFormat("{0} __V{1};\n", name, v.Index);
            // }
            var code = body.Instructions;

            // gather all branching targets
            var labels = new Dictionary<int,OpLabel>();
            foreach (var instr in code) {
                switch (instr.OpCode.OperandType) {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget: {
                        var target = (instr.Operand as Instruction).Offset;
                        var label = new OpLabel(this.rcount++);
                        labels.Add(target, label);
                        break;
                    }
                    default:
                        break;
                }
            }



            var stack = new Stack<TypedResultOpCode>();
            foreach (var instr in code) {
                // emit label if current instruction is a branching target
                if (labels.ContainsKey(instr.Offset)) {
                    funcdef.Add(labels[instr.Offset]);
                }
                // main handler for current instruction
                switch (instr.OpCode.Code)
                {
                case Code.Nop:
                    // just for the fun of it...
                    funcdef.Add(new OpNop());
                    break;
                case Code.Dup: {
                    stack.Push(stack.Peek());
                    break;
                }
                case Code.Ldc_I4_0: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 0));
                    break;
                }
                case Code.Ldc_I4_1: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 1));
                    break;
                }
                case Code.Ldc_I4_2: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 2));
                    break;
                }
                case Code.Ldc_I4_3: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 3));
                    break;
                }
                case Code.Ldc_I4_4: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 4));
                    break;
                }
                case Code.Ldc_I4_5: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 5));
                    break;
                }
                case Code.Ldc_I4_6: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 6));
                    break;
                }
                case Code.Ldc_I4_7: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 7));
                    break;
                }
                case Code.Ldc_I4_8: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), 8));
                    break;
                }
                case Code.Ldc_I4_M1: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), -1));
                    break;
                }
                case Code.Ldc_I4: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), (int)instr.Operand));
                    break;
                }
                case Code.Ldc_I4_S: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<int>(), (sbyte)instr.Operand));
                    break;
                }
                case Code.Ldc_I8: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<long>(), (long)instr.Operand));
                    break;
                }
                case Code.Ldc_R4: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<float>(), (float)instr.Operand));
                    break;
                }
                case Code.Ldc_R8: {
                    stack.Push(new OpConstant(SpirConstantCallback, (NumericTypeOpCode)GetTypeOpCode<double>(), (double)instr.Operand));
                    break;
                }
                case Code.Ldarg_0: {
                    stack.Push(param[0]);
                    break;
                }
                case Code.Ldarg_1: {
                    stack.Push(param[1]);
                    break;
                }
                case Code.Ldarg_2: {
                    stack.Push(param[2]);
                    break;
                }
                case Code.Ldarg_3: {
                    stack.Push(param[3]);
                    break;
                }
                case Code.Ldarg:
                case Code.Ldarg_S: {
                    var arg = instr.Operand as ParameterDefinition;
                    stack.Push(param[arg.Index]);
                    break;
                }
                case Code.Ldloc_0: {
                    stack.Push(vars[0]);
                    break;
                }
                case Code.Ldloc_1: {
                    stack.Push(vars[1]);
                    break;
                }
                case Code.Ldloc_2: {
                    stack.Push(vars[2]);
                    break;
                }
                case Code.Ldloc_3: {
                    stack.Push(vars[3]);
                    break;
                }
                case Code.Ldloc:
                case Code.Ldloc_S: {
                    var loc = instr.Operand as VariableDefinition;
                    stack.Push(vars[loc.Index]);
                    break;
                }
                case Code.Ldelem_Any:
                case Code.Ldelem_I:
                case Code.Ldelem_I1:
                case Code.Ldelem_I2:
                case Code.Ldelem_I4:
                case Code.Ldelem_U1:
                case Code.Ldelem_U2:
                case Code.Ldelem_U4:
                case Code.Ldelem_I8:
                case Code.Ldelem_R4:
                case Code.Ldelem_R8: {
                    var idx = stack.Pop();
                    var arr = stack.Pop();
                    var addr = new OpAccessChain(this.rcount++, arr, idx);
                    var elem = new OpLoad(this.rcount++, addr);
                    funcdef.Add(addr);
                    funcdef.Add(elem);
                    stack.Push(elem);
                    break;
                }
                case Code.Ldind_I:
                case Code.Ldind_I1:
                case Code.Ldind_I2:
                case Code.Ldind_I4:
                case Code.Ldind_I8:
                case Code.Ldind_R4:
                case Code.Ldind_R8:
                case Code.Ldobj: {
                    var addr = stack.Pop();
                    var elem = new OpLoad(this.rcount++, addr);
                    funcdef.Add(elem);
                    stack.Push(elem);
                    break;
                }
                case Code.Ldarga:
                case Code.Ldarga_S: {
                    var arg = instr.Operand as ParameterDefinition;
                    var ptr = new OpVariable(this.rcount++, new OpTypePointer(SpirTypeIdCallback, StorageClass.Function, GetTypeOpCode(arg)));
                    funcdef.Add(ptr);
                    stack.Push(ptr);
                    funcdef.Add(new OpStore(ptr, param[arg.Index]));
                    break;
                }
                case Code.Ldloca:
                case Code.Ldloca_S: {
                    var loc = instr.Operand as VariableDefinition;
                    var ptr = new OpVariable(this.rcount++, new OpTypePointer(SpirTypeIdCallback, StorageClass.Function, GetTypeOpCode(loc)));
                    funcdef.Add(ptr);
                    stack.Push(ptr);
                    funcdef.Add(new OpStore(ptr, vars[loc.Index]));
                    break;
                }
                case Code.Ldelema: {
                    var idx = stack.Pop();
                    var arr = stack.Pop();
                    var addr = new OpAccessChain(this.rcount++, arr, idx);
                    funcdef.Add(addr);
                    stack.Push(addr);
                    break;
                }
                case Code.Localloc: {
                    throw new NotSupportedException();
                }
                case Code.Stloc_0: {
                    vars[0] = stack.Pop();
                    break;
                }
                case Code.Stloc_1: {
                    vars[1] = stack.Pop();
                    break;
                }
                case Code.Stloc_2: {
                    vars[2] = stack.Pop();
                    break;
                }
                case Code.Stloc_3: {
                    vars[2] = stack.Pop();
                    break;
                }
                case Code.Stloc:
                case Code.Stloc_S: {
                    var loc = instr.Operand as VariableDefinition;
                    vars[loc.Index] = stack.Pop();
                    break;
                }
                case Code.Stelem_Any:
                case Code.Stelem_I:
                case Code.Stelem_I1:
                case Code.Stelem_I2:
                case Code.Stelem_I4:
                case Code.Stelem_I8:
                case Code.Stelem_R4:
                case Code.Stelem_R8: {
                    var val = stack.Pop();
                    var idx = stack.Pop();
                    var arr = stack.Pop();
                    var ptr = new OpAccessChain(this.rcount++, arr, idx);
                    funcdef.Add(ptr);
                    funcdef.Add(new OpStore(ptr, val));
                    break;
                }
                case Code.Stind_I:
                case Code.Stind_I1:
                case Code.Stind_I2:
                case Code.Stind_I4:
                case Code.Stind_I8:
                case Code.Stind_R4:
                case Code.Stind_R8:
                case Code.Stobj: {
                    var val = stack.Pop();
                    var ptr = stack.Pop();
                    funcdef.Add(new OpStore(ptr, val));
                    break;
                }
                case Code.Conv_I:
                case Code.Conv_Ovf_I:
                case Code.Conv_Ovf_I_Un: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(IntPtr));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Conv_I1:
                case Code.Conv_Ovf_I1:
                case Code.Conv_Ovf_I1_Un: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(SByte));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Conv_I2:
                case Code.Conv_Ovf_I2:
                case Code.Conv_Ovf_I2_Un: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(Int16));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Conv_I4:
                case Code.Conv_Ovf_I4:
                case Code.Conv_Ovf_I4_Un: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(Int32));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Conv_I8:
                case Code.Conv_Ovf_I8:
                case Code.Conv_Ovf_I8_Un: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(Int64));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Conv_U:
                case Code.Conv_Ovf_U:
                case Code.Conv_Ovf_U_Un:
                    throw new NotSupportedException();
                    // stack.Push(new Conv(typeof(UIntPtr), stack.Pop()));
                    // break;
                case Code.Conv_U1:
                case Code.Conv_Ovf_U1:
                case Code.Conv_Ovf_U1_Un:
                    throw new NotSupportedException();
                    // stack.Push(new Conv(typeof(Byte), stack.Pop()));
                    // break;
                case Code.Conv_U2:
                case Code.Conv_Ovf_U2:
                case Code.Conv_Ovf_U2_Un:
                    throw new NotSupportedException();
                    // stack.Push(new Conv(typeof(UInt16), stack.Pop()));
                    // break;
                case Code.Conv_U4:
                case Code.Conv_Ovf_U4:
                case Code.Conv_Ovf_U4_Un:
                    throw new NotSupportedException();
                    // stack.Push(new Conv(typeof(UInt32), stack.Pop()));
                    // break;
                case Code.Conv_U8:
                case Code.Conv_Ovf_U8:
                case Code.Conv_Ovf_U8_Un:
                    throw new NotSupportedException();
                    // stack.Push(new Conv(typeof(UInt64), stack.Pop()));
                    // break;
                case Code.Conv_R4: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(Single));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Conv_R8: {
                    var arg = stack.Pop();
                    var op = GetConversionOpCode(arg, typeof(Double));
                    funcdef.Add(op);
                    stack.Push(op);
                    break;
                }
                case Code.Add:
                case Code.Add_Ovf:
                case Code.Add_Ovf_Un: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpIAdd(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else if (l.ResultType is OpTypeFloat && r.ResultType is OpTypeFloat) {
                        var tl = l.ResultType as OpTypeFloat;
                        var tr = r.ResultType as OpTypeFloat;
                        if (tl.Width < tr.Width) {
                            l = new OpFConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpFConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpFAdd(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'add' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Sub:
                case Code.Sub_Ovf:
                case Code.Sub_Ovf_Un: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpISub(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else if (l.ResultType is OpTypeFloat && r.ResultType is OpTypeFloat) {
                        var tl = l.ResultType as OpTypeFloat;
                        var tr = r.ResultType as OpTypeFloat;
                        if (tl.Width < tr.Width) {
                            l = new OpFConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpFConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpFSub(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'sub' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Mul:
                case Code.Mul_Ovf:
                case Code.Mul_Ovf_Un: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpIMul(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else if (l.ResultType is OpTypeFloat && r.ResultType is OpTypeFloat) {
                        var tl = l.ResultType as OpTypeFloat;
                        var tr = r.ResultType as OpTypeFloat;
                        if (tl.Width < tr.Width) {
                            l = new OpFConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpFConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpFMul(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'mul' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Div: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpSDiv(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else if (l.ResultType is OpTypeFloat && r.ResultType is OpTypeFloat) {
                        var tl = l.ResultType as OpTypeFloat;
                        var tr = r.ResultType as OpTypeFloat;
                        if (tl.Width < tr.Width) {
                            l = new OpFConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpFConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpFDiv(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'div' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Div_Un: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpUConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpUConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpUDiv(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    // else if (l.ResultType is OpTypeFloat && r.ResultType is OpTypeFloat) {
                    //     var tl = l.ResultType as OpTypeFloat;
                    //     var tr = r.ResultType as OpTypeFloat;
                    //     if (tl.Width < tr.Width) {
                    //         l = new OpFConvert(this.rcount++, tr, l);
                    //         funcdef.Add(l);
                    //     }
                    //     else if (tr.Width < tl.Width) {
                    //         r = new OpFConvert(this.rcount++, tl, r);
                    //         funcdef.Add(r);
                    //     }
                    //     var op = new OpFDiv(this.rcount++, l, r);
                    //     funcdef.Add(op);
                    //     stack.Push(op);
                    // }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'div' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.And: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpBitwiseAnd(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'and' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Or: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpBitwiseOr(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'or' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Xor: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpBitwiseXor(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'xor' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Shl: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpShiftLeftLogical(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'xor' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Shr: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpShiftRightArithmetic(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'xor' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Shr_Un: {
                    var r = stack.Pop();
                    var l = stack.Pop();
                    if (l.ResultType is OpTypeInt && r.ResultType is OpTypeInt) {
                        var tl = l.ResultType as OpTypeInt;
                        var tr = r.ResultType as OpTypeInt;
                        if (tl.Width < tr.Width) {
                            l = new OpSConvert(this.rcount++, tr, l);
                            funcdef.Add(l);
                        }
                        else if (tr.Width < tl.Width) {
                            r = new OpSConvert(this.rcount++, tl, r);
                            funcdef.Add(r);
                        }
                        var op = new OpShiftRightLogical(this.rcount++, l, r);
                        funcdef.Add(op);
                        stack.Push(op);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible types for 'xor' operation: {0} and {1}.", l.ResultType.GetType().Name, r.ResultType.GetType().Name));
                    }
                    break;
                }
                case Code.Call: {
                    var mref = instr.Operand as MethodReference;
                    var mdef = mref.Resolve();
                    var tdef = mdef.DeclaringType;
                    var name = tdef.FullName + "." + mdef.Name;
                    var nargs = mref.Parameters.Count;
                    if (mdef.HasThis && !mdef.ExplicitThis) {
                        nargs++;
                    }
                    switch (name)
                    {
                    case "OpenCl.Cl.GetGlobalId": {
                        TypedResultOpCode sym;
                        if (!this.imports.TryGetValue(name, out sym)) {
                            // type of import symbol
                            var t = new OpTypePointer(SpirTypeIdCallback, StorageClass.UniformConstant,
                                        new OpTypeVector(SpirTypeIdCallback, 3,
                                            new OpTypeInt(SpirTypeIdCallback, 64)));
                            // import symbol
                            sym = new OpVariable(this.rcount++, t);
                            this.imports.Add(name, sym);
                            // import decorations
                            this.decorations.Add(new OpDecorateBuiltIn(sym, BuiltIn.GlobalInvocationId));
                            this.decorations.Add(new OpDecorateConstant(sym));
                            this.decorations.Add(new OpDecorateLinkageAttributes(sym, LinkageType.Import, BuiltIn.GlobalInvocationId));
                        }
                        var ld = new OpLoad(this.rcount++, sym);
                        var op = new OpVectorExtractDynamic(this.rcount++, ld, stack.Pop());
                        funcdef.Add(ld);
                        funcdef.Add(op);
                        stack.Push(op);
                        break;
                    }
                    case "OpenCl.int2.op_Addition":
                    case "OpenCl.int3.op_Addition":
                    case "OpenCl.int4.op_Addition":
                    case "OpenCl.int8.op_Addition":
                    case "OpenCl.int16.op_Addition": {
                        TypedResultOpCode v = stack.Pop();
                        TypedResultOpCode u = stack.Pop();
                        var op = new OpIAdd(this.rcount++, u, v);
                        funcdef.Add(op);
                        stack.Push(op);
                        break;
                    }
                    default:
                        throw new CompilerException(String.Format("Unsupported call to '{0}'.", name));
                    }
                //     var args = new AstNode[nargs];
                //     for (var i=nargs-1; i>=0; i--) {
                //         args[i] = stack.Pop();
                //     }
                //     if (mdef.IsConstructor) {
                //         var tdef = mdef.DeclaringType;
                //         if (tdef.IsValueType) {
                //             // Note: this assumes that the struct constructor
                //             // is compatible with C-style compound literals.
                //             this.builder.Append("*(");
                //             args[0].Accept(this.printer);
                //             this.builder.AppendFormat(") = ({0}){{ ", typeMap[tdef.FullName]);
                //             for (int i=1; i<nargs; i++) {
                //                 if (i > 1) {
                //                     this.builder.Append(", ");
                //                 }
                //                 args[i].Accept(this.printer);
                //             }
                //             this.builder.AppendLine(" };");
                //         }
                //     }
                //     else {
                //         var rtype = CliType.FromType(mdef.ReturnType);
                //         switch (name)
                //         {
                //         case "op_Addition":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Add, args[0], args[1]));
                //             break;
                //         case "op_Subtraction":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Sub, args[0], args[1]));
                //             break;
                //         case "op_Multiply":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Mul, args[0], args[1]));
                //             break;
                //         case "op_Division":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Div, args[0], args[1]));
                //             break;
                //         case "op_Equality":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Eq, args[0], args[1]));
                //             break;
                //         case "op_Inequality":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Neq, args[0], args[1]));
                //             break;
                //         case "op_LessThan":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Lt, args[0], args[1]));
                //             break;
                //         case "op_LessThanOrEqual":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Le, args[0], args[1]));
                //             break;
                //         case "op_GreaterThan":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Gt, args[0], args[1]));
                //             break;
                //         case "op_GreaterThanOrEqual":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Ge, args[0], args[1]));
                //             break;
                //         case "op_BitwiseAnd":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.And, args[0], args[1]));
                //             break;
                //         case "op_BitwiseOr":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Or, args[0], args[1]));
                //             break;
                //         case "op_ExclusiveOr":
                //             stack.Push(new BinaryOp(rtype, BinaryOpCode.Xor, args[0], args[1]));
                //             break;
                //         case "op_OnesComplement":
                //             stack.Push(new UnaryOp(UnaryOpCode.Not, args[0]));
                //             break;
                //         default:
                //             if (mdef.HasThis && name.StartsWith("get_")) {
                //                 // Note: this assumes that the property getter is a valid
                //                 // C-style field reference.
                //                 stack.Push(new FieldRef(rtype, name.Substring(4), args[0]));
                //             }
                //             else if (mdef.HasThis && name.StartsWith("set_")) {
                //                 // Note: this assumes that the property setter is a valid
                //                 // C-style field reference.
                //                 this.builder.Append("(*");
                //                 args[0].Accept(this.printer);
                //                 this.builder.AppendFormat(").{0} = ", name.Substring(4));
                //                 args[1].Accept(this.printer);
                //                 this.builder.AppendLine(";");
                //             }
                //             else {
                //                 stack.Push(new Call(rtype, name, args));
                //             }
                //             break;
                //         }
                //     }
                    break;
                }
                case Code.Br:
                case Code.Br_S: {
                    var target = (instr.Operand as Instruction).Offset;
                    funcdef.Add(new OpBranch(labels[target]));
                    break;
                }
                case Code.Brfalse:
                case Code.Brfalse_S: {
                    var arg = stack.Pop();
                    if (!(arg.ResultType is OpTypeInt)) {
                        throw new CompilerException(String.Format("Incompatible operand for 'brtrue' instruction: {0}.", arg.ResultType.GetType().Name));
                    }
                    var zero = new OpConstant(SpirConstantCallback, arg.ResultType as NumericTypeOpCode, 0);
                    var cond = new OpIEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), arg, zero);
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Brtrue:
                case Code.Brtrue_S: {
                    var arg = stack.Pop();
                    if (!(arg.ResultType is OpTypeInt)) {
                        throw new CompilerException(String.Format("Incompatible operand for 'brtrue' instruction: {0}.", arg.ResultType.GetType().Name));
                    }
                    var zero = new OpConstant(SpirConstantCallback, arg.ResultType as NumericTypeOpCode, 0);
                    var cond = new OpINotEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), arg, zero);
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Beq:
                case Code.Beq_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpIEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFOrdEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'beq' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Bne_Un:
                case Code.Bne_Un_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpINotEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordNotEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'beq' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Blt:
                case Code.Blt_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpSLessThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordLessThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'blt' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Blt_Un:
                case Code.Blt_Un_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpULessThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordLessThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'blt' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Ble:
                case Code.Ble_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpSLessThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordLessThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'ble' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Ble_Un:
                case Code.Ble_Un_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpULessThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordLessThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'ble' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Bgt:
                case Code.Bgt_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpSGreaterThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordGreaterThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'bgt' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Bgt_Un:
                case Code.Bgt_Un_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpUGreaterThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordGreaterThan(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'bgt' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Bge:
                case Code.Bge_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpSGreaterThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordGreaterThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'bge' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Bge_Un:
                case Code.Bge_Un_S: {
                    var v = stack.Pop();
                    var u = stack.Pop();
                    var cond = (TypedResultOpCode)null;
                    if (u.ResultType is OpTypeInt && v.ResultType is OpTypeInt) {
                        cond = new OpUGreaterThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else if (u.ResultType is OpTypeFloat && v.ResultType is OpTypeFloat) {
                        cond = new OpFUnordGreaterThanEqual(this.rcount++, new OpTypeBool(SpirTypeIdCallback), u, v);
                    }
                    else {
                        throw new CompilerException(String.Format("Incompatible operands for 'bge' instruction: {0} and {1}.", u.ResultType.GetType().Name, v.ResultType.GetType().Name));
                    }
                    var lt = labels[(instr.Operand as Instruction).Offset];
                    var lf = new OpLabel(this.rcount++);
                    var br = new OpBranchConditional(cond, lt, lf);
                    funcdef.AddRange(new SpirOpCode[] { cond, br, lf});
                    break;
                }
                case Code.Ret:
                    if (method.ReturnType.FullName != "System.Void") {
                        funcdef.Add(new OpReturnValue(stack.Pop()));
                    }
                    else {
                        funcdef.Add(new OpReturn());
                    }
                    break;
                default:
                    throw new CompilerException(String.Format("Unsupported opcode: {0}.", instr.OpCode));
                }
            }
            funcdef.Add(new OpFunctionEnd());
            this.functions.Add(funcdef);
        }

        private void Emit(Stream output)
        {
            // serialize all 'function' instructions
            MemoryStream op_func = new MemoryStream();
            // functions
            foreach (var func in this.functions) {
                foreach (var op in func) {
                    op.Emit(op_func);
                }
            }

            // serialize all 'variable' instructions
            MemoryStream op_vars = new MemoryStream();
            // imports
            foreach (var op in this.imports) {
                op.Value.Emit(op_vars);
            }
            // exports
            // ...
            // constants
            foreach (var op in this.constants.Keys) {
                op.Emit(op_vars);
            }

            // serialize all 'declaration' instructions
            MemoryStream op_decl = new MemoryStream();
            // capabilities
            new OpCapability(Capability.Addresses).Emit(op_decl);
            new OpCapability(Capability.Linkage).Emit(op_decl);
            new OpCapability(Capability.Kernel).Emit(op_decl);
            new OpCapability(Capability.Int64).Emit(op_decl);
            // import OpenCL extended instruction set
            // see: https://www.khronos.org/registry/spir-v/specs/1.0/OpenCL.ExtendedInstructionSet.100.html
            new OpExtInstImport(this.rcount++, "OpenCL.std").Emit(op_decl);
            // memory model
            new OpMemoryModel(AddressingModel.Physical64, MemoryModel.Simple).Emit(op_decl);
            // entry points
            foreach (var op in this.entryPoints) {
                op.Emit(op_decl);
            }
            // decorations
            foreach (var op in this.decorations) {
                op.Emit(op_decl);
            }
            // types
            for (var i=0; i<this.types_list.Count; i++) {
                var op = this.types_list[i];
                op.Emit(op_decl);
            }

            var bound = this.rcount;
            // SPIR-V magic number
            output.WriteByte(0x03);
            output.WriteByte(0x02);
            output.WriteByte(0x23);
            output.WriteByte(0x07);
            // SPIR-V version number
            output.WriteByte(0x00);
            output.WriteByte(0x00);
            output.WriteByte(0x01);
            output.WriteByte(0x00);
            // SPIR-V generator number
            output.WriteByte(0x00);
            output.WriteByte(0x00);
            output.WriteByte(0x00);
            output.WriteByte(0x00);
            // SPIR-V max ID
            output.WriteIntLE(bound);
            // reserved (must be zero)
            output.WriteIntLE(0x00);
            // 'declarations'
            op_decl.Seek(0, SeekOrigin.Begin);
            op_decl.CopyTo(output);
            // 'variables'
            op_vars.Seek(0, SeekOrigin.Begin);
            op_vars.CopyTo(output);
            // 'functions'
            op_func.Seek(0, SeekOrigin.Begin);
            op_func.CopyTo(output);
            // sanity check
            if (bound != this.rcount) {
                throw new Exception(String.Format("Invalid result ID bound: expected {0}, found {1}", bound, this.rcount));
            }
        }

        private void Run(Stream output)
        {
            while (this.queue.Count > 0) {
                Parse(this.queue.Dequeue());
            }
            Emit(output);
        }

        public static void EmitKernel(string module, string type, string method, Stream output)
        {
            using (var _module = ModuleDefinition.ReadModule(module, new ReaderParameters { AssemblyResolver = new SimpleAssemblyResolver() }))
            {
                TypeDefinition _type = _module.Types.SingleOrDefault(ti => ti.FullName == type);
                if (_type == null) {
                    Console.WriteLine("*** Error: could not find type '{0}'.", type);
                    return;
                }
                MethodDefinition _method = _type.Methods.Single(mi => mi.Name == method);
                if (_method == null) {
                    Console.WriteLine("*** Error: could not find method '{0}'.", method);
                    return;
                }
                EmitKernel(/*_module, _type,*/ _method, output);
            }
        }

        public static void EmitKernel(/*ModuleDefinition module, TypeDefinition type,*/ MethodDefinition method, Stream output)
        {
            SpirCompiler compiler = new SpirCompiler(/*module, type,*/ method);
            compiler.Run(output);
        }
    }
}

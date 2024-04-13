using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters mrcp = (ModbusReadCommandParameters)CommandParameters;
            byte[] recVal = new byte[12];
            recVal[0] = BitConverter.GetBytes(mrcp.TransactionId)[1];
            recVal[1] = BitConverter.GetBytes(mrcp.TransactionId)[0];
            recVal[2] = BitConverter.GetBytes(mrcp.ProtocolId)[1];
            recVal[3] = BitConverter.GetBytes(mrcp.ProtocolId)[0];
            recVal[4] = BitConverter.GetBytes(mrcp.Length)[1];
            recVal[5] = BitConverter.GetBytes(mrcp.Length)[0];
            recVal[6] = mrcp.UnitId;
            recVal[7] = mrcp.FunctionCode;
            recVal[8] = BitConverter.GetBytes(mrcp.StartAddress)[1];
            recVal[9] = BitConverter.GetBytes(mrcp.StartAddress)[0];
            recVal[10] = BitConverter.GetBytes(mrcp.Quantity)[1];
            recVal[11] = BitConverter.GetBytes(mrcp.Quantity)[0];

            return recVal;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusReadCommandParameters mrcp = (ModbusReadCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> recVal = new Dictionary<Tuple<PointType, ushort>, ushort>();
            if(response.Length <= 9)
            {
                Console.WriteLine("[ERROR] Message is not valid.");
            }
            else
            {
                for(int i = 0; i < response[8]; i += 2)
                {
                    Tuple<PointType, ushort> tmp = Tuple.Create(PointType.ANALOG_OUTPUT, mrcp.StartAddress);
                    byte[] byte_array = new byte[2];
                    byte_array[0] = response[10 + i];
                    byte_array[1] = response[9 + i];
                    recVal.Add(tmp, BitConverter.ToUInt16(byte_array, 0));
                }
            }
            return recVal;
        }
    }
}
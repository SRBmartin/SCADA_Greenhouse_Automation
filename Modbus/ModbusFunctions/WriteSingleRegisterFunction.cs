using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters mwcp = (ModbusWriteCommandParameters)CommandParameters;
            byte[] recVal = new byte[12];
            recVal[0] = BitConverter.GetBytes(mwcp.TransactionId)[1];
            recVal[1] = BitConverter.GetBytes(mwcp.TransactionId)[0];
            recVal[2] = BitConverter.GetBytes(mwcp.ProtocolId)[1];
            recVal[3] = BitConverter.GetBytes(mwcp.ProtocolId)[0];
            recVal[4] = BitConverter.GetBytes(mwcp.Length)[1];
            recVal[5] = BitConverter.GetBytes(mwcp.Length)[0];
            recVal[6] = mwcp.UnitId;
            recVal[7] = mwcp.FunctionCode;
            recVal[8] = BitConverter.GetBytes(mwcp.OutputAddress)[1];
            recVal[9] = BitConverter.GetBytes(mwcp.OutputAddress)[0];
            recVal[10] = BitConverter.GetBytes(mwcp.Value)[1];
            recVal[11] = BitConverter.GetBytes(mwcp.Value)[0];
            return recVal;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusWriteCommandParameters mwcp = (ModbusWriteCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> recVal = new Dictionary<Tuple<PointType, ushort>, ushort>();
            if(response.Length <= 9)
            {
                Console.WriteLine("[ERROR] Message is not valid.");
            }
            else
            {
                for(int i = 0; i < response[8]; i += 2)
                {
                    Tuple<PointType, ushort> tmp = Tuple.Create(PointType.ANALOG_OUTPUT, mwcp.OutputAddress);
                    byte[] byte_array = new byte[2];
                    byte_array[0] = response[10];
                    byte_array[1] = response[9];
                    recVal.Add(tmp, BitConverter.ToUInt16(byte_array, 0));
                }
            }
            return recVal;
        }
    }
}
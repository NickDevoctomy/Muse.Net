using Muse.Net.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muse.Net.Services;
using Muse.Net.Models;

namespace Muse.Net.Client
{
    public class MuseClient : IMuseClient
    {
        public event EventHandler<MuseClientNotifyTelemetryEventArgs> NotifyTelemetry;
        public event EventHandler<MuseClientNotifyAccelerometerEventArgs> NotifyAccelerometer;
        public event EventHandler<MuseClientNotifyGyroscopeEventArgs> NotifyGyroscope;
        public event EventHandler<MuseClientNotifyEegEventArgs> NotifyEeg;

        private readonly IBluetoothClient<Channel, IGattCharacteristic> _bluetoothClient;
        private readonly IMuseDataParserService _museDataParserService;

        public bool Connected => _bluetoothClient.Connected;

        public MuseClient(
            IBluetoothClient<Channel, IGattCharacteristic> bluetoothClient,
            IMuseDataParserService museDataParserService)
        {
            _bluetoothClient = bluetoothClient;
            _bluetoothClient.OnGattValueChanged = OnGattValueChanged;
            _museDataParserService = museDataParserService;
        }

        public Task<bool> Connect(ulong deviceAddress)
        {
            return _bluetoothClient.Connect(
                deviceAddress,
                MuseGuid.PRIMARY_SERVICE,
                new KeyValuePair<Channel, Guid>(Channel.Control, MuseGuid.CONTROL),
                new KeyValuePair<Channel, Guid>(Channel.Accelerometer, MuseGuid.ACELEROMETER),
                new KeyValuePair<Channel, Guid>(Channel.Gyroscope, MuseGuid.GYROSCOPE),
                new KeyValuePair<Channel, Guid>(Channel.Telemetry, MuseGuid.TELEMETRY),
                new KeyValuePair<Channel, Guid>(Channel.EEG_TP9, MuseGuid.EEG_TP9),
                new KeyValuePair<Channel, Guid>(Channel.EEG_TP10, MuseGuid.EEG_TP10),
                new KeyValuePair<Channel, Guid>(Channel.EEG_AF7, MuseGuid.EEG_AF7),
                new KeyValuePair<Channel, Guid>(Channel.EEG_AF8, MuseGuid.EEG_AF8),
                new KeyValuePair<Channel, Guid>(Channel.EEG_AUX, MuseGuid.EEG_AUX));
        }

        public async Task Disconnect()
        {
            await _bluetoothClient.UnsubscribeAll();
            await _bluetoothClient.Disconnect();
        }

        public async Task Subscribe(params Channel[] channels)
        {
            foreach (var channel in channels)
            {
                await SubscribeToChannel(channel);
            }
        }

        public Task UnsubscribeAll()
        {
            return _bluetoothClient.UnsubscribeAll();
        }

        public async Task Resume()
        {
            await _bluetoothClient.Characteristics[Channel.Control].WriteCommand(MuseCommand.RESUME);
        }

        public async Task Start()
        {
            await _bluetoothClient.Characteristics[Channel.Control].WriteCommand(MuseCommand.START);
        }

        public async Task Pause()
        {
            await _bluetoothClient.Characteristics[Channel.Control].WriteCommand(MuseCommand.PAUSE);
        }

        public Task<bool> SubscribeToChannel(Channel channel)
        {
            return _bluetoothClient.SubscribeToChannel(channel);
        }

        public Task<bool> UnsubscribeFromChannel(Channel channel)
        {
            return _bluetoothClient.UnsubscribeFromChannel(channel);
        }

        protected void OnGattValueChanged(
            Channel characteristicKeyType,
            byte[] data)
        {
            switch (characteristicKeyType)
            {
                case Channel.Telemetry: TriggerNotifyTelemetry(data); break;
                case Channel.Accelerometer: TriggerNotifyAccelerometer(data); break;
                case Channel.Gyroscope: TriggerNotifyGyroscope(data); break;
                case Channel.EEG_AF7:
                case Channel.EEG_AF8:
                case Channel.EEG_TP9:
                case Channel.EEG_TP10:
                case Channel.EEG_AUX: TriggerNotifyEeg(characteristicKeyType, data); break;
            }
        }

        public async Task<Telemetry> ReadTelemetryAsync()
        {
            var bytes = await _bluetoothClient.SingleChannelEventAsync(Channel.Telemetry);
            if (bytes != null)
            {
                return _museDataParserService.Telemetry(bytes);
            }
            else return null;
        }

        private void TriggerNotifyEeg(
            Channel channel,
            ReadOnlySpan<byte> bytes)
        { 
            var eeg = _museDataParserService.Encefalogram(bytes);
            NotifyEeg?.Invoke(this, new MuseClientNotifyEegEventArgs { Channel = channel, Encefalogram = eeg });
        }

        private void TriggerNotifyTelemetry(ReadOnlySpan<byte> bytes)
        {
            var telemetry = _museDataParserService.Telemetry(bytes);
            NotifyTelemetry?.Invoke(this, new MuseClientNotifyTelemetryEventArgs { Telemetry = telemetry });
        }

        private void TriggerNotifyAccelerometer(ReadOnlySpan<byte> bytes)
        {
            var accelerometer = _museDataParserService.Accelerometer(bytes);
            NotifyAccelerometer?.Invoke(this, new MuseClientNotifyAccelerometerEventArgs { Accelerometer = accelerometer });
        }

        private void TriggerNotifyGyroscope(ReadOnlySpan<byte> bytes)
        {
            var gyroscope = _museDataParserService.Gyroscope(bytes);
            NotifyGyroscope?.Invoke(this, new MuseClientNotifyGyroscopeEventArgs { Gyroscope = gyroscope });
        }
    }
}

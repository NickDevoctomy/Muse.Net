using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Muse.Net.Client;
using Muse.Net.Models.Enums;
using Muse.Net.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Muse.Net.UnitTests.Client
{
    [TestClass]
    public class MuseClientTests
    {
        [TestMethod]
        public async Task GivenMuseClient_WhenConnect_ThenBluetoothClientConnectCalled_AndChannelsConfigured()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var deviceId = (ulong)100;
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.Setup(x => x.Connect(
                It.IsAny<ulong>(),
                It.IsAny<System.Guid>(),
                It.IsAny<KeyValuePair<Channel, System.Guid>[]>()))
                .ReturnsAsync(true);

            // Act
            var result = await sut.Connect(deviceId);

            // Assert
            Assert.IsTrue(result);
            mockBluetoothClient.Verify(x => x.Connect(
                It.Is<ulong>(y => y == deviceId),
                It.Is<System.Guid>(y => y == MuseGuid.PRIMARY_SERVICE),
                It.Is<KeyValuePair<Channel, System.Guid>[]>(y => AssertConnectionChannels(y))), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenDisconnect_ThenBluetoothClientDisconnected()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.Setup(x => x.UnsubscribeAll())
                .Returns(Task.CompletedTask);

            mockBluetoothClient.Setup(x => x.Disconnect())
                .Returns(Task.CompletedTask);

            // Act
            await sut.Disconnect();

            // Assert
            mockBluetoothClient.Verify(x => x.UnsubscribeAll(), Times.Once);
            mockBluetoothClient.Verify(x => x.Disconnect(), Times.Once);
        }

        [TestMethod]
        public async Task GivenChannels_WhenSubscribe_ThenBluetoothClientSubscribeToChannelCalled()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var channels = new Channel[]
            {
                Channel.Accelerometer,
                Channel.Control,
                Channel.EEG_AF7,
                Channel.EEG_AF8,
                Channel.EEG_AUX,
                Channel.EEG_TP10,
                Channel.EEG_TP9,
                Channel.Gyroscope,
                Channel.Telemetry
            };
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.Setup(x => x.SubscribeToChannel(
                It.IsAny<Channel>()))
                .ReturnsAsync(true);

            // Act
            await sut.Subscribe(channels);

            // Assert
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Accelerometer)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Control)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_AF7)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_AF8)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_AUX)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_TP10)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_TP9)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Gyroscope)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Telemetry)), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenUnsubscribeAll_ThenBluetoothClientUnsubscribeAll()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.Setup(x => x.UnsubscribeAll())
                .Returns(Task.CompletedTask);

            // Act
            await sut.UnsubscribeAll();

            // Assert
            mockBluetoothClient.Verify(x => x.UnsubscribeAll(), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenResume_ThenCharacteristicControlWriteCommand()
        {
            // Arrange
            var mockGattCharacteristic = new Mock<IGattCharacteristic>();
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.SetupGet(x => x.Characteristics[Channel.Control])
                .Returns(mockGattCharacteristic.Object);

            // Act
            await sut.Resume();

            // Assert
            mockGattCharacteristic.Verify(x => x.WriteCommand(
                It.Is<string>(y => y == MuseCommand.RESUME)), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenPause_ThenCharacteristicControlWriteCommand()
        {
            // Arrange
            var mockGattCharacteristic = new Mock<IGattCharacteristic>();
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.SetupGet(x => x.Characteristics[Channel.Control])
                .Returns(mockGattCharacteristic.Object);

            // Act
            await sut.Pause();

            // Assert
            mockGattCharacteristic.Verify(x => x.WriteCommand(
                It.Is<string>(y => y == MuseCommand.PAUSE)), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenStart_ThenCharacteristicControlWriteCommand()
        {
            // Arrange
            var mockGattCharacteristic = new Mock<IGattCharacteristic>();
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.SetupGet(x => x.Characteristics[Channel.Control])
                .Returns(mockGattCharacteristic.Object);

            // Act
            await sut.Start();

            // Assert
            mockGattCharacteristic.Verify(x => x.WriteCommand(
                It.Is<string>(y => y == MuseCommand.START)), Times.Once);
        }

        [TestMethod]
        public async Task GivenChannel_WhenSubscribeToChannel_ThenBluetoothClientSubscribeToChannel_AndResultReturned()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var channel = Channel.EEG_AF7;
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.Setup(x => x.SubscribeToChannel(
                It.IsAny<Channel>()))
                .ReturnsAsync(true);

            // Act
            var result = await sut.SubscribeToChannel(channel);

            // Assert
            Assert.IsTrue(result);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.IsAny<Channel>()), Times.Once);
        }

        [TestMethod]
        public async Task GivenChannel_WhenUnsubscribeFromChannel_ThenBluetoothClientUnsubscribeFromChannel_AndResultReturned()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var channel = Channel.EEG_AF7;
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                null);

            mockBluetoothClient.Setup(x => x.UnsubscribeFromChannel(
                It.IsAny<Channel>()))
                .ReturnsAsync(true);

            // Act
            var result = await sut.UnsubscribeFromChannel(channel);

            // Assert
            Assert.IsTrue(result);
            mockBluetoothClient.Verify(x => x.UnsubscribeFromChannel(
                It.IsAny<Channel>()), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenReadTelemetryAsync_ThenBluetoothClientSingleChannelEventAsync_AndParserTelemetry_AndResultReturned()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var mockMuseDataParser = new Mock<IMuseDataParserService>();
            var channelData = new byte[] { 1, 2, 3 };
            var parsedTelemetry = new Models.Telemetry();
            var sut = new MuseClient(
                mockBluetoothClient.Object,
                mockMuseDataParser.Object);

            mockBluetoothClient.Setup(x => x.SingleChannelEventAsync(
                It.IsAny<Channel>()))
                .ReturnsAsync(channelData);

            mockMuseDataParser.Setup(x => x.Telemetry(
                It.IsAny<byte[]>()))
                .Returns(parsedTelemetry);

            // Act
            var result = await sut.ReadTelemetryAsync();

            // Assert
            Assert.AreEqual(result, parsedTelemetry);
            mockBluetoothClient.Verify(x => x.SingleChannelEventAsync(
                It.Is<Channel>(y => y == Channel.Telemetry)), Times.Once);
            mockMuseDataParser.Verify(x => x.Telemetry(
                It.Is<byte[]>(y => y == channelData)), Times.Once);
        }


        [TestMethod]
        public void GivenMuseClient_WhenBluetoothClientOnGattValueChanged_AndTelemetryChannel_ThenParserTelemetry_AndNotified()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var mockMuseDataParser = new Mock<IMuseDataParserService>();
            var channel = Channel.Telemetry;
            var data = new byte[] { 1, 2, 3, 4 };
            var parsedTelemetry = new Models.Telemetry();
            var onGattValueChanged = default(Action<Channel, byte[]>);
            mockBluetoothClient.SetupSet(p => p.OnGattValueChanged = It.IsAny<Action<Channel, byte[]>>())
                .Callback<Action<Channel, byte[]>>(value => onGattValueChanged = value);

            var sut = new MuseClient(
                mockBluetoothClient.Object,
                mockMuseDataParser.Object);

            var notified = false;
            sut.NotifyTelemetry += (s, a) =>
            {
                notified = a.Telemetry == parsedTelemetry;
            };

            mockMuseDataParser.Setup(x => x.Telemetry(
                It.IsAny<byte[]>()))
                .Returns(parsedTelemetry);

            // Act
            onGattValueChanged(
                channel,
                data);

            // Assert
            mockMuseDataParser.Verify(x => x.Telemetry(
                It.IsAny<byte[]>()), Times.Once);
            Assert.IsTrue(notified);
        }

        [TestMethod]
        public void GivenMuseClient_WhenBluetoothClientOnGattValueChanged_AndAccelerometerChannel_ThenParserAccelerometer_AndNotified()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var mockMuseDataParser = new Mock<IMuseDataParserService>();
            var channel = Channel.Accelerometer;
            var data = new byte[] { 1, 2, 3, 4 };
            var parsedAccelerometer = new Models.Accelerometer();
            var onGattValueChanged = default(Action<Channel, byte[]>);
            mockBluetoothClient.SetupSet(p => p.OnGattValueChanged = It.IsAny<Action<Channel, byte[]>>())
                .Callback<Action<Channel, byte[]>>(value => onGattValueChanged = value);

            var sut = new MuseClient(
                mockBluetoothClient.Object,
                mockMuseDataParser.Object);

            var notified = false;
            sut.NotifyAccelerometer += (s, a) =>
            {
                notified = a.Accelerometer == parsedAccelerometer;
            };

            mockMuseDataParser.Setup(x => x.Accelerometer(
                It.IsAny<byte[]>()))
                .Returns(parsedAccelerometer);

            // Act
            onGattValueChanged(
                channel,
                data);

            // Assert
            mockMuseDataParser.Verify(x => x.Accelerometer(
                It.IsAny<byte[]>()), Times.Once);
            Assert.IsTrue(notified);
        }

        [TestMethod]
        public void GivenMuseClient_WhenBluetoothClientOnGattValueChanged_AndGyroscopeChannel_ThenParserGyroscope_AndNotified()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var mockMuseDataParser = new Mock<IMuseDataParserService>();
            var channel = Channel.Gyroscope;
            var data = new byte[] { 1, 2, 3, 4 };
            var parsedGyroscope = new Models.Gyroscope();
            var onGattValueChanged = default(Action<Channel, byte[]>);
            mockBluetoothClient.SetupSet(p => p.OnGattValueChanged = It.IsAny<Action<Channel, byte[]>>())
                .Callback<Action<Channel, byte[]>>(value => onGattValueChanged = value);

            var sut = new MuseClient(
                mockBluetoothClient.Object,
                mockMuseDataParser.Object);

            var notified = false;
            sut.NotifyGyroscope += (s, a) =>
            {
                notified = a.Gyroscope == parsedGyroscope;
            };

            mockMuseDataParser.Setup(x => x.Gyroscope(
                It.IsAny<byte[]>()))
                .Returns(parsedGyroscope);

            // Act
            onGattValueChanged(
                channel,
                data);

            // Assert
            mockMuseDataParser.Verify(x => x.Gyroscope(
                It.IsAny<byte[]>()), Times.Once);
            Assert.IsTrue(notified);
        }

        [TestMethod]
        public void GivenMuseClient_WhenBluetoothClientOnGattValueChanged_AndAuxChannel_ThenParserGyroscope_AndNotified()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var mockMuseDataParser = new Mock<IMuseDataParserService>();
            var channel = Channel.EEG_AUX;
            var data = new byte[] { 1, 2, 3, 4 };
            var parsedEncefalogram = new Models.Encefalogram();
            var onGattValueChanged = default(Action<Channel, byte[]>);
            mockBluetoothClient.SetupSet(p => p.OnGattValueChanged = It.IsAny<Action<Channel, byte[]>>())
                .Callback<Action<Channel, byte[]>>(value => onGattValueChanged = value);

            var sut = new MuseClient(
                mockBluetoothClient.Object,
                mockMuseDataParser.Object);

            var notified = false;
            sut.NotifyEeg += (s, a) =>
            {
                notified = a.Encefalogram == parsedEncefalogram;
            };

            mockMuseDataParser.Setup(x => x.Encefalogram(
                It.IsAny<byte[]>()))
                .Returns(parsedEncefalogram);

            // Act
            onGattValueChanged(
                channel,
                data);

            // Assert
            mockMuseDataParser.Verify(x => x.Encefalogram(
                It.IsAny<byte[]>()), Times.Once);
            Assert.IsTrue(notified);
        }

        private bool AssertConnectionChannels(KeyValuePair<Channel, System.Guid>[] connectionChannels)
        {
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Accelerometer));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Control));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_AF7));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_AF8));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_AUX));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_TP10));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_TP9));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Gyroscope));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Telemetry));
            return true;
        }
    }
}

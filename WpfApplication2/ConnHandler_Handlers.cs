using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eriver.Network;
using log4net;
using EriverTrackers;

namespace Eriver.GUIServer
{
    static class ConnHandler_Handlers
    {
        public static IDictionary<byte, Func<Message>> Messages;

        static ConnHandler_Handlers() 
        {
            var assembly = Assembly.GetExecutingAssembly();
            Messages = assembly
                .GetTypes()
                .Where(t => typeof(Message).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Keys = t.GetCustomAttributes(typeof(AcceptsAttribute), true)
                            .Cast<AcceptsAttribute>().Select(attr => attr.MessageId),
                    Value = (Func<Message>)Expression.Lambda(
                            Expression.Convert(Expression.New(t), typeof(Message)))
                            .Compile()
                })
                .SelectMany(o => o.Keys.Select(key => new { Key = key, o.Value }))
                .ToDictionary(o => o.Key, v => v.Value);
            //will give you a runtime error when created if more 
            //than one class accepts the same message id, <= useful test case?
        }

        public sealed class AcceptsAttribute : Attribute
        {
            public AcceptsAttribute(byte messageId) { MessageId = messageId; }

            public byte MessageId { get; private set; }
        }

        public abstract class Message
        {
            public abstract void Accept(ConnectionHandler ch, EriverProtocol packet);
            public virtual EriverProtocol Create() { return new EriverProtocol(); }
        }

        [Accepts((byte) Command.GetPoint)]
        public class GetPointMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.Listen = !ch.Listen;
            }
        }

        [Accepts((byte)Command.StartCalibration)]
        public class StartCalMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                XConfSettings xconf = new XConfSettings(
                Properties.Settings.Default.ScreenHeight,
                Properties.Settings.Default.ScreenWidth,
                Properties.Settings.Default.TrackerDeltaX,
                Properties.Settings.Default.TrackerDeltaY,
                Properties.Settings.Default.TrackerDeltaZ,
                Properties.Settings.Default.TrackerDeltaAngle);
                ch.GetTracker().SetXConfig(xconf, packet.StartCalibration.Angle);
                ch.GetTracker().StartCalibration(defaultAction(ch, packet));
            }
        }

        [Accepts((byte)Command.EndCalibration)]
        public class EndCalMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.GetTracker().EndCalibration(defaultAction(ch, packet));
            }
        }

        [Accepts((byte)Command.ClearCalibration)]
        public class ClearCalMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.GetTracker().ClearCalibration(defaultAction(ch, packet));
            }
        }

        [Accepts((byte)Command.AddPoint)]
        public class AddPointMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.GetTracker().AddPoint(packet.AddPoint.X, packet.AddPoint.Y , defaultAction(ch, packet));
            }
        }

        [Accepts((byte)Command.Unavailable)]
        public class UnavaliableMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.Send(packet);
            }
        }

        [Accepts((byte)Command.Name)]
        public class NameMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.GetTracker().GetName(delegate(int error, int result)
                {
                    packet.Name.Value = (error != 0) ? (byte) result : (byte) 0;
                    ch.Send(packet);
                });
            }
        }

        [Accepts((byte)Command.Fps)]
        public class FpsMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.GetTracker().GetRate(delegate(int error, int result)
                {
                    packet.Fps.Value = (error != 0) ? (uint)result : 0;
                    ch.Send(packet);
                });
            }
        }

        [Accepts((byte)Command.KeepAlive)]
        public class KeepAliveMessage : Message
        {
            public override void Accept(ConnectionHandler ch, EriverProtocol packet)
            {
                ch.Send(packet);
            }
        }

        static Trackers.TrackerCallback defaultAction(ConnectionHandler ch, EriverProtocol packet)
        {
            return delegate(int error, int result)
            {
                if (error != 0 || result == 0)
                {
                    packet.Kind = Command.Unavailable;
                }
                ch.Send(packet);
            };
        }
    }
}

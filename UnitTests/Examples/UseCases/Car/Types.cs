using System;

namespace NGineer.UnitTests.Examples.UseCases.Car
{
    public class Car
    {
        public SerialNumber VIN;
        public Wheel[] Wheels;
        public string Registration;
        public int DistanceSinceLastOilChange;
        public DateTime? LastOilChange;
    }

    public class Wheel
    {
        public bool IsFlat;
    }

    public class SerialNumber
    {
        public string Serial;
    }
}
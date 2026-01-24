namespace Safi.Exceptions
{
    public class RoomNumberAlreadyExistsException : Exception
    {
        public int RoomNumber { get; }
        public int DepartmentId { get; }

        public RoomNumberAlreadyExistsException(int roomNumber, int departmentId)
            : base($"Room number {roomNumber} already exists in department {departmentId}.")
        {
            RoomNumber = roomNumber;
            DepartmentId = departmentId;
        }
    }
}

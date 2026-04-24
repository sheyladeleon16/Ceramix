namespace Ceramix.Domain.Enums;

public enum PieceStatus
{
    InProgress  = 0,
    Drying      = 1,
    Dried       = 2,
    Fired       = 3,
    Completed   = 4,
    Damaged     = 5
}

public enum CeramicTechnique
{
    HandBuilding    = 0,
    WheelThrowing   = 1,
    SlipCasting     = 2,
    Coiling         = 3,
    Pinching        = 4,
    Slab            = 5,
    Sculpting       = 6
}

public enum FiringType
{
    Bisque      = 0,   // Primera quema (sin esmalte)
    Glaze       = 1,   // Segunda quema (con esmalte)
    Raku        = 2,   // Técnica raku
    Pit         = 3,   // Quema en fosa
    Reduction   = 4    // Reducción de oxígeno
}

public enum FiringStatus
{
    Pending     = 0,
    InProgress  = 1,
    Completed   = 2,
    Cancelled   = 3
}

public enum MaterialType
{
    Clay        = 0,
    Glaze       = 1,
    Pigment     = 2,
    Tool        = 3,
    Kiln        = 4,
    Other       = 5
}

public enum DeliveryStatus
{
    Pending     = 0,
    Delivered   = 1,
    Returned    = 2
}

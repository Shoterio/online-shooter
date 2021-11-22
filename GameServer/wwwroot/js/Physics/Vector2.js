
function Vector2(x = 0, y = 0) {

    const that = this;

    that._x = x;
    that.GetX = function () {
        return that._x;
    };
    that.SetX = function(value) {

        that._x = value;
        that._length = undefined;
        that._lengthSquared = undefined;
    };

    that._y = y;
    that.GetY = function () {
        return that._y;
    };
    that.SetY = function (value) {
        
        that._y = value;
        that._length = undefined;
        that._lengthSquared = undefined;
    };

    that.Set = function (x, y) {

        that.SetX(x);
        that.SetY(y);
    };

    that.CopyFrom = function (vector) {

        that.SetX(vector.GetX());
        that.SetY(vector.GetY());
        that._length = vector._length;
        that._lengthSquared = vector._lengthSquared;
    };

    that.LengthSquared = function () {

        if (typeof that._lengthSquared === "undefined")
            that._lengthSquared = Math.pow(that.GetX(), 2) + Math.pow(that.GetY(), 2);

        return that._lengthSquared;
    };

    that.Length = function () {

        if (typeof that._length === "undefined")
            that._length = Math.sqrt(that.LengthSquared());

        return that._length;
    };

    that.Normalize = function () {

        if (that.IsDegenerated())
            throw "Degenerated vector given. Cannot normalize zero vector.";

        let vector = Vector2.Clone(that);
        vector = Vector2.Divide(vector, that.Length());
        return vector;
    };

    that.SafeNormalize = function () {

        try {
            that.Normalize();
        }
        catch (err) {
            return new Vector2();
        }
    };

    that.IsDegenerated = function () {

        return that.LengthSquared() === 0;
    };

    // Helpers
    that.Equals = function (otherVector) {

        if (!otherVector)
            return false;

        return that.GetX() === otherVector.GetX() && that.GetY() === otherVector.GetY();
    };

    that.ToString = function () {

        return `<${that.GetX()} ${that.GetY()}>`;
    };
};

Vector2.Clone = function(vector) {

    const copiedVector = new Vector2(vector.GetX(), vector.GetY());

    copiedVector._length = vector._length;
    copiedVector._lengthSquared = vector._lengthSquared;

    return copiedVector;
};

Vector2.Dot = function (vectorA, vectorB) {

    return (vectorA.GetX() * vectorB.GetX()) + (vectorA.GetY() * vectorB.GetY());
};

Vector2.Norm2 = function (vector) {

    const sumsqr = Math.pow(vector.GetX(), 2) + Math.pow(vector.GetY(), 2);
    return Math.sqrt(sumsqr);
};

Vector2.Similarity = function (vectorA, vectorB) {

    return Vector2.Dot(vectorA, vectorB) / Vector2.Norm2(vectorA) / Vector2.Norm2(vectorB);
};

Vector2.DistanceSquared = function (vectorA, vectorB) {

    return Math.pow(vectorA.GetX() - vectorB.GetX(), 2) + Math.pow(vectorA.GetY() - vectorB.GetY(), 2);
};

Vector2.Distance = function (vectorA, vectorB) {

    return Math.sqrt(Vector2.DistanceSquared(vectorA, vectorB));
};

Vector2.Normalize = function (vector) {

    return vector.Normalize();
};

Vector2.RadianToVector2 = function (radian) {

    return new Vector2(Math.cos(radian), Math.sin(radian));
};

Vector2.Vector2ToRadian = function (vector) {

    return Math.atan2(vector.GetY(), vector.GetX());
};

Vector2.AngleBetweenVectors = function (vectorA, vectorB) {

    if (vectorA.IsDegenerated() || vectorB.IsDegenerated())
        throw "One of given vectors is degenerated. Cannot calculate angle between them";

    const sin = vectorB.GetX() * vectorA.GetY() - vectorA.GetX() * vectorB.GetY();
    const cos = vectorB.GetX() * vectorA.GetX() + vectorB.GetY() * vectorA.GetY();

    return Math.atan2(sin, cos);
};

// Mathematical operations
Vector2.Add = function (vectorA, vectorB) {

    return new Vector2(vectorA.GetX() + vectorB.GetX(), vectorA.GetY() + vectorB.GetY());
};

Vector2.Subtract = function (vectorA, vectorB) {

    return new Vector2(vectorA.GetX() - vectorB.GetX(), vectorA.GetY() - vectorB.GetY());
};

Vector2.Multiply = function (vector, multiplier) {

    return new Vector2(vector.GetX() * multiplier, vector.GetY() * multiplier);
};

Vector2.Divide = function (vector, divider) {

    if (divider === 0)
        throw "Division by 0";

    return Vector2.Multiply(vector, 1.0 / divider);
};

Vector2.LEFT_VECTOR = () => new Vector2(-1, 0);
Vector2.RIGHT_VECTOR = () => new Vector2(1, 0);
Vector2.UP_VECTOR = () => new Vector2(0, -1);
Vector2.DOWN_VECTOR = () => new Vector2(0, 1);
Vector2.ZERO_VECTOR = () => new Vector2(0, 0);

// Export module
if (typeof module !== 'undefined' && module.hasOwnProperty('exports')) {
    module.exports = Vector2;
}
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
// ABOUT THIS FILE:
// spiro.models defines a set of classes that correspond directly to the JSON representations returned by Restful Objects
// resources.  These classes provide convenient methods for navigating the contents of those representations, and for
// following links to other resources.
// null  version of shims
var Spiro;
(function (Spiro) {
    var ModelShim = (function () {
        function ModelShim(object) {
            this.attributes = object;
        }
        ModelShim.prototype.url = function () {
            return "";
        };

        ModelShim.prototype.get = function (attributeName) {
            return this.attributes[attributeName];
        };
        ModelShim.prototype.set = function (attributeName, value, options) {
            this.attributes[attributeName] = value;
        };
        return ModelShim;
    })();
    Spiro.ModelShim = ModelShim;

    // base class for all representations that can be directly loaded from the server
    var HateoasModelBaseShim = (function (_super) {
        __extends(HateoasModelBaseShim, _super);
        function HateoasModelBaseShim(object) {
            _super.call(this, object);
            this.hateoasUrl = "";
            this.method = "GET";
            this.suffix = "";
        }
        HateoasModelBaseShim.prototype.url = function () {
            return (this.hateoasUrl || _super.prototype.url.call(this)) + this.suffix;
        };
        HateoasModelBaseShim.prototype.onError = function (map, statusCode, warnings) {
        };
        HateoasModelBaseShim.prototype.preFetch = function () {
        };
        return HateoasModelBaseShim;
    })(ModelShim);
    Spiro.HateoasModelBaseShim = HateoasModelBaseShim;

    var ArgumentMap = (function (_super) {
        __extends(ArgumentMap, _super);
        function ArgumentMap(map, parent, id) {
            _super.call(this, map);
            this.id = id;
        }
        ArgumentMap.prototype.onChange = function () {
        };
        ArgumentMap.prototype.onError = function (map, statusCode, warnings) {
        };
        return ArgumentMap;
    })(HateoasModelBaseShim);
    Spiro.ArgumentMap = ArgumentMap;

    var CollectionShim = (function () {
        function CollectionShim(object) {
        }
        CollectionShim.prototype.url = function () {
        };

        CollectionShim.prototype.add = function (models, options) {
            this.models = this.models || [];

            for (var i = 0; i < models.length; i++) {
                var m = new this.model(models[i]);
                this.models.push(m);
            }
        };
        return CollectionShim;
    })();
    Spiro.CollectionShim = CollectionShim;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.models.shims.js.map

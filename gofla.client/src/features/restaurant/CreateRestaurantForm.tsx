import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "react-router-dom";
import { restaurantApi, useCreateRestaurantMutation } from "../../app/api/restaurant/restaurantApi";
import { AddressAutocomplete } from "../address/AddressAutoComplete";
import { CreateRestaurantSchema, type CreateRestaurantFormValues } from "../../utils/validators/createRestaurantSchema";
import { Input } from "../../app/layout/ui/Input";
import { Button } from "../../app/layout/ui/Button";
import { toast } from "react-toastify";

// type AddressAutocompleteResult = {
//   street: string;
//   city: string;
//   state?: string;
//   postalCode?: string;
//   countryCode: string;
//   latitude: number;
//   longitude: number;
// };


export default function CreateRestaurantForm() {
  const navigate = useNavigate();
  const [createRestaurant, { isLoading }] = useCreateRestaurantMutation();

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
    clearErrors,
  } = useForm<CreateRestaurantFormValues>({
    resolver: zodResolver(CreateRestaurantSchema),
    // defaultValues: {
    //   deliveryFee: 0,
    //   estimatedDeliveryTime: 30,
    //   deliveryRadiusKm: 5,
    // },
  });

  const normalizePhone = (phone: string) =>
  phone.replace(/[^\d+]/g, "");


  const onSubmit = async (data: CreateRestaurantFormValues) => {
    try {
      normalizePhone(data.phone);
      const result = await createRestaurant(data).unwrap();
      toast.success("Restaurant Added!")
      
      navigate(`/restaurants/${result.data.id}`);
      //dispatch(restaurantApi.util.invalidateTags(['Restaurant']));
      console.log(result.data.id); 
    } catch (error) {
      toast.error("Failed Adding Restaurant!")   
    }
    
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="space-y-5 max-w-xl mx-auto px-4 sm:px-0"
    >
      <h1 className="flex font-serif font-bold text-2xl mt-4.5 justify-center">Register Your Restaurant</h1>
      <div>
        <label className="font-serif text-lg block mb-3">
          Restaurant Name
           <span className="text-red-500 ml-0.5">*</span>
        </label>
        <Input
          {...register("name")}
          placeholder="Restaurant name"
        />
        {errors.name && <p className="text-red-500 text-sm">{errors.name.message}</p>}
      </div>

      <div>
        <label className="font-serif text-lg block mb-3">
          Restaurant Description
           <span className="text-red-500 ml-0.5">*</span>
        </label>
        <textarea
          {...register("description")}
          placeholder="Describe your restaurant"
          className="textarea  w-full rounded-xl border border-gray-300 bg-amber-50 
          px-4 py-3 text-gray-800 placeholder-gray-400 shadow-sm
          focus:border-amber-400 focus:bg-white focus:ring-2 
          focus:ring-amber-300 transition-all duration-200 outline-none"
        />
        {errors.description && (
          <p className="text-red-500 text-sm">{errors.description.message}</p>
        )}
      </div>

      <div>
        <label className="font-serif text-lg block mb-3">
          Phone Number
           <span className="text-red-500 ml-0.5">*</span>
          </label>
        <Input
          {...register("phone")}
          placeholder="Phone number"
          className="input"
        />
        {errors.phone && <p className="text-red-500 text-sm">{errors.phone.message}</p>}
      </div>

      <div>
        <label className="font-serif text-lg block mb-3">
          Delivery Fee
           <span className="text-red-500 ml-0.5">*</span>
          </label>
        <Input
          type="number"
          min={0}
          step="0.1"
          {...register("deliveryFee", { valueAsNumber: true })}
          placeholder="Delivery fee"
          className="input"
        />
        {errors.deliveryFee && (
          <p className="text-red-500 text-sm">{errors.deliveryFee.message}</p>
        )}
      </div>

      <div>
        <label className="font-serif text-lg block mb-3">
          Delivery Time
           <span className="text-red-500 ml-0.5">*</span>
          </label>
        <Input
          type="number"
          min={1}
          {...register("estimatedDeliveryTime", { valueAsNumber: true })}
          placeholder="Estimated delivery time (minutes)"
          className="input"
        />
        {errors.estimatedDeliveryTime && (
          <p className="text-red-500 text-sm">{errors.estimatedDeliveryTime.message}</p>
        )}
      </div>

      <div>
        <label className="font-serif text-lg block mb-3">
          Delivery Radius
           <span className="text-red-500 ml-0.5">*</span>
          </label>
        <Input
          type="number"
          min={1}
          {...register("deliveryRadiusKm", { valueAsNumber: true })}
          placeholder="Delivery radius (km)"
          className="input"
        />
        {errors.deliveryRadiusKm && (
          <p className="text-red-500 text-sm">{errors.deliveryRadiusKm.message}</p>
        )}
      </div>

      <div>
        <label className="font-serif text-lg block mb-3">
          Address
           <span className="text-red-500 ml-0.5">*</span>
          </label>
        <AddressAutocomplete
          onSelect={(address) => {
            setValue(
              "addressDto",
              {
                label: "Main",
                street: address.street,
                city: address.city,
                state: address.state,
                postalCode: address.postalCode,
                countryCode: address.countryCode,
                latitude: address.latitude,
                longitude: address.longitude,
              },
              { shouldValidate: true }
            );
            clearErrors("addressDto");
          }}
        />
        {errors.addressDto && (
          <p className="text-red-500 text-sm">Valid address is required</p>
        )}
      </div>

      <Button
        disabled={isLoading}
       variant="amber"
       className=" w-full justify-center font-serif text-lg mb-5 mt-3"
      >
        {isLoading ? "Creating..." : "Create Restaurant"}
      </Button>
    </form>
  );
}
function dispatch(arg0: any) {
  throw new Error("Function not implemented.");
}

